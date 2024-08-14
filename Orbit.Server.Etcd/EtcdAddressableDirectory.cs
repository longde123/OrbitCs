using System.Text;
using Etcdserverpb;
using Google.Protobuf;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Orbit.Server.Mesh;
using Orbit.Shared.Addressable;
using Orbit.Shared.Proto;
using Orbit.Util.Concurrent;
using Orbit.Util.Time;

namespace Orbit.Server.Etcd;

public class EtcdAddressableDirectory : BaseAsyncMap<NamespacedAddressableReference, AddressableLease>,
    IAddressableDirectory
{
    private readonly EtcdAddressableDirectoryConfig _config;
    private readonly byte[] _allKey = Encoding.ASCII.GetBytes("\0");
    private readonly GrpcChannel _channel;
    private readonly Clock _clock;
    private readonly string _keyPrefix = "addressable";
    private readonly KV.KVClient _kvClient;
    private readonly AtomicReference<bool> _lastHealthCheck = new(false);
    private readonly AtomicReference<long> _lastHealthCheckTime = new(0);
    private readonly Lease.LeaseClient _leaseClient;
    private readonly ILogger _logger;

    public EtcdAddressableDirectory(EtcdAddressableDirectoryConfig config, Clock clock, LoggerFactory loggerFactory)
    {
        _config = config;
        _clock = clock;
        _logger = loggerFactory.CreateLogger<EtcdAddressableDirectory>();
        _channel = EtcdAddressableDirectoryConfig.CreateAuthenticatedChannel(config.Url);
        _kvClient = new KV.KVClient(_channel);
        _leaseClient = new Lease.LeaseClient(_channel);
    }


    public async Task Tick()
    {
    }

    public async Task<bool> IsHealthy()
    {
        if (_lastHealthCheckTime.Get() + 5000 > _clock.CurrentTime)
        {
            return _lastHealthCheck.Get();
        }

        try
        {
            _lastHealthCheckTime.AtomicSet(l => _clock.CurrentTime);
            await Task.Delay(3000);
            await GetLease(Timestamp.FromDateTime(DateTime.UtcNow));
            _lastHealthCheck.AtomicSet(l => true);
            return true;
        }
        catch (TimeoutException et)
        {
            _lastHealthCheck.AtomicSet(l => false);
            return false;
        }
        catch (Exception e)
        {
            _lastHealthCheck.AtomicSet(l => false);
            return false;
        }
    }

    public override async Task<long> Count()
    {
        var request = new RangeRequest
        {
            Key = ByteString.CopyFrom(Encoding.ASCII.GetBytes(_keyPrefix)),
            SortTarget = RangeRequest.Types.SortTarget.Key,
            SortOrder = RangeRequest.Types.SortOrder.Descend,
            CountOnly = true,
            RangeEnd = ByteString.CopyFrom(_allKey)
        };
        var response = await _kvClient.RangeAsync(request);
        return response.Count;
    }

    public override async Task<AddressableLease?> Get(NamespacedAddressableReference key)
    {
        // 创建 RangeRequest 对象，用于获取单个键的值

        var rangeRequest = new RangeRequest
        {
            Key = ByteString.CopyFrom(ToByteKey(key))
        };
        var response = await _kvClient.RangeAsync(rangeRequest);
        var value = response.Kvs.FirstOrDefault()?.Value;
        if (value != null)
        {
            return AddressableLeaseProto.Parser.ParseFrom(value.ToByteArray()).ToAddressableLease();
        }

        return null;
    }

    public override async Task<bool> Remove(NamespacedAddressableReference key)
    {
        await _kvClient.DeleteRangeAsync(new DeleteRangeRequest
        {
            Key = ByteString.CopyFrom(ToByteKey(key))
        });
        return true;
    }

    public override async Task<bool> CompareAndSet(NamespacedAddressableReference key, AddressableLease? initialValue,
        AddressableLease? newValue)
    {
        var byteKey = ToByteKey(key);
        var entry = (await _kvClient.RangeAsync(new RangeRequest
            {
                Key = ByteString.CopyFrom(byteKey)
            })).Kvs
            .FirstOrDefault();
        var oldValue = entry?.Value?.ToByteArray()?.Length > 0
            ? AddressableLeaseProto.Parser.ParseFrom(entry.Value.ToByteArray()).ToAddressableLease()
            : null;

        if (initialValue == oldValue)
        {
            if (newValue != null)
            {
                await _kvClient.PutAsync(new PutRequest
                {
                    Key = ByteString.CopyFrom(byteKey),
                    Value = ByteString.CopyFrom(newValue.ToAddressableLeaseProto().ToByteArray()),
                    Lease = (await GetLease(newValue.ExpiresAt)).ID
                });
            }
            else
            {
                await _kvClient.DeleteRangeAsync(new DeleteRangeRequest
                {
                    Key = ByteString.CopyFrom(byteKey)
                });
            }

            return true;
        }

        return false;
    }


    private async Task<LeaseGrantResponse> GetLease(Timestamp time)
    {
        // 创建租约请求
        var leaseRequest = new LeaseGrantRequest
        {
            TTL = _clock.Until(time.ToDateTime()).Seconds, // 租约的持续时间，单位为秒
            ID = 0 // 租约ID，0表示让服务端分配
        };
        var lease = await _leaseClient.LeaseGrantAsync(leaseRequest);
        return lease;
    }

    private byte[] ToByteKey(NamespacedAddressableReference reference)
    {
        return
            Encoding.ASCII.GetBytes(
                $"{_keyPrefix}/{reference.Namespace}/{reference.AddressableReference.Type}/{reference.AddressableReference.Key}"
            );
    }
}