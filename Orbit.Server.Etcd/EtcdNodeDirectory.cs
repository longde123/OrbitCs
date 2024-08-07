/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using System.Text;
using Etcdserverpb;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Orbit.Server.Mesh;
using Orbit.Shared.Mesh;
using Orbit.Shared.Proto;
using Orbit.Util.Concurrent;
using Orbit.Util.Time;

namespace Orbit.Server.Etcd;

public class EtcdNodeDirectory : BaseAsyncMap<NodeId, NodeInfo>, INodeDirectory
{
    private readonly byte[] _allKey = Encoding.UTF8.GetBytes("\0");
    private readonly Clock _clock;
    private readonly string _keyPrefix = "node";


    private readonly KV.KVClient _kvClient;
    private readonly AtomicReference<bool> _lastHealthCheck = new(false);

    private readonly AtomicReference<long> _lastHealthCheckTime = new(0);
    private readonly Lease.LeaseClient _leaseClient;

    private readonly ILogger _logger;

    public EtcdNodeDirectory(EtcdNodeDirectoryConfig config, Clock clock)
    {
        var channel = EtcdAddressableDirectoryConfig.CreateAuthenticatedChannel(config.Url);
        _kvClient = new KV.KVClient(channel);
        _leaseClient = new Lease.LeaseClient(channel);
        this._clock = clock;
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
            await Task.Run(() => GetLease(Timestamp.Now()));
            _lastHealthCheck.AtomicSet(l => true);
            return true;
        }
        catch (TimeoutException te)
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

    public override async Task<NodeInfo?> Get(NodeId key)
    {
        var rangeRequest = new RangeRequest
        {
            Key = ByteString.CopyFrom(ToByteKey(key))
        };
        var response = await _kvClient.RangeAsync(rangeRequest);
        var entry = response.Kvs.FirstOrDefault();
        var dValue = entry?.Value?.ToByteArray()?.Length > 0
            ? NodeInfoProto.Parser.ParseFrom(entry.Value.ToByteArray()).ToNodeInfo()
            : null;
        return dValue;
    }

    public override async Task<bool> Remove(NodeId key)
    {
        await _kvClient.DeleteRangeAsync(new DeleteRangeRequest { Key = ByteString.CopyFrom(ToByteKey(key)) });
        return true;
    }

    public override async Task<bool> CompareAndSet(NodeId key, NodeInfo? initialValue, NodeInfo? newValue)
    {
        var byteKey = ToByteKey(key);
        var entry = (await _kvClient.RangeAsync(new RangeRequest { Key = ByteString.CopyFrom(byteKey) })).Kvs
            .FirstOrDefault();
        var oldValue = entry?.Value?.ToByteArray()?.Length > 0
            ? NodeInfoProto.Parser.ParseFrom(entry.Value.ToByteArray()).ToNodeInfo()
            : null;

        if (initialValue.Equals(oldValue))
        {
            if (newValue != null)
            {
                await _kvClient.PutAsync(new PutRequest
                {
                    Key = ByteString.CopyFrom(byteKey),
                    Value = ByteString.CopyFrom(newValue.ToNodeInfoProto().ToByteArray()),
                    Lease = (await GetLease(newValue.Lease.ExpiresAt)).ID
                });
            }
            else
            {
                await _kvClient.DeleteRangeAsync(new DeleteRangeRequest { Key = ByteString.CopyFrom(byteKey) });
            }

            return true;
        }

        return false;
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

    public async Task<IEnumerable<(NodeId, NodeInfo)>> Entries()
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

        var ienumerable = response.Kvs.Select(kv =>
            (FromByteKey(kv.Key.ToByteArray()), NodeInfoProto.Parser.ParseFrom(kv.Value.ToByteArray()).ToNodeInfo())
        );
        return ienumerable;
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

    private byte[] ToByteKey(NodeId nodeId)
    {
        return
            Encoding.UTF8.GetBytes($"{_keyPrefix}/{nodeId.Namespace}/{nodeId.Key}");
    }

    private NodeId FromByteKey(byte[] keyBytes)
    {
        var keyString = Encoding.UTF8.GetString(keyBytes);
        var parts = keyString.Split('/');

        return new NodeId(parts[2], parts[1]);
    }
}