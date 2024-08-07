using Microsoft.Extensions.Logging;
using Orbit.Server.Service;
using Orbit.Shared.Mesh;
using Orbit.Util.Concurrent;
using Orbit.Util.Time;

namespace Orbit.Server.Mesh;

public class LocalNodeInfo : IHealthCheck
{
    public static string ManagementNamespace = "management";
    private readonly Clock _clock;
    private readonly ClusterManager _clusterManager;
    private readonly AtomicReference<NodeInfo> _infoRef = new(null);
    private readonly ILogger _logger;
    private readonly LocalServerInfo _serverInfo;

    public LocalNodeInfo(ClusterManager clusterManager, Clock clock, LocalServerInfo serverInfo,
        ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<LocalNodeInfo>();
        this._clusterManager = clusterManager;
        this._clock = clock;
        this._serverInfo = serverInfo;
    }


    public NodeInfo Info
    {
        get
        {
            var info = _infoRef.Get();
            if (info == null)
            {
                throw new InvalidOperationException("LocalNodeInfo not initialized.");
            }

            return info;
        }
    }

    public async Task<bool> IsHealthy()
    {
        return Info.NodeStatus == NodeStatus.Active;
    }

    public async Task UpdateInfo(Func<NodeInfo, NodeInfo> body)
    {
        var it = await _clusterManager.UpdateNode(Info.Id, it =>
        {
            if (it == null)
            {
                throw new InvalidOperationException($"LocalNodeInfo not present in directory. {Info.Id}");
            }

            return body(it);
        });
        _infoRef.AtomicSet(i => it);
    }

    public async Task Start()
    {
        await Join();
    }

    public async Task Join(NodeId? nodeId = null, NodeStatus nodeStatus = NodeStatus.Starting)
    {
        var it = await _clusterManager.JoinCluster(ManagementNamespace, new NodeCapabilities(), _serverInfo.Url,
            nodeStatus, nodeId);
        _logger.LogInformation($"Joined cluster as ({it.Id})");
        _infoRef.AtomicSet(i => it);
    }

    public async Task Tick()
    {
        if (_clock.InPast(Info.Lease.RenewAt.ToDateTime()))
        {
            //todo 
            // try
            // {
            var it = await _clusterManager.RenewLease(Info.Id, Info.Lease.ChallengeToken, Info.Capabilities);

            _infoRef.AtomicSet(i => it);
            // }
            // catch (InvalidNodeId e)
            // {
            //     logger.LogInformation("Failed to renew lease, rejoining cluster.");
            //     await Join(Info.Id, NodeStatus.ACTIVE);
            // }
        }
    }
}