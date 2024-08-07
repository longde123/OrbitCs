using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Orbit.Server.Mesh.Graph;
using Orbit.Shared.Mesh;
using Orbit.Shared.OrException;
using Orbit.Util.Concurrent;
using Orbit.Util.Misc;
using Orbit.Util.Time;

namespace Orbit.Server.Mesh;

public class ClusterManager
{
    private readonly Clock _clock;
    private readonly ConcurrentDictionary<NodeId, NodeInfo> _clusterNodes;
    private readonly OrbitServerConfig _config;
    private readonly LeaseDuration _leaseExpiration;
    private readonly ILogger _logger;
    private readonly INodeDirectory _nodeDirectory;
    private readonly AtomicReference<DefaultDirectedGraph<NodeId>> _nodeGraph;

    public ClusterManager(OrbitServerConfig config, Clock clock, INodeDirectory nodeDirectory,
        ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ClusterManager>();
        this._config = config;
        this._clock = clock;
        this._nodeDirectory = nodeDirectory;
        _leaseExpiration = config.NodeLeaseDuration;
        _clusterNodes = new ConcurrentDictionary<NodeId, NodeInfo>();
        _nodeGraph =
            new AtomicReference<DefaultDirectedGraph<NodeId>>(null);
    }

    public List<NodeInfo> GetAllNodes()
    {
        return _clusterNodes.Values
            .Where(node => _clock.InFuture(node.Lease.ExpiresAt.ToDateTime()))
            .ToList();
    }

    public async Task Tick()
    {
        var allNodes = await _nodeDirectory.Entries();
        _clusterNodes.Clear();
        foreach (var node in allNodes)
        {
            _clusterNodes.TryAdd(node.Item1, node.Item2);
        }

        BuildGraph();
    }

    public async Task<NodeInfo> JoinCluster(string nameSpace, NodeCapabilities capabilities, string? url = null,
        NodeStatus nodeStatus = NodeStatus.Stopped, NodeId? nodeId = null)
    {
        var newNodeId = nodeId ?? NodeId.Generate(nameSpace);

        var lease = new NodeLease
        {
            ChallengeToken = RngUtils.RandomString(64),
            ExpiresAt = _clock.Now().Add(_leaseExpiration.ExpiresIn).ToTimestamp(),
            RenewAt = _clock.Now().Add(_leaseExpiration.RenewIn).ToTimestamp()
        };

        var info = new NodeInfo
        {
            Id = newNodeId,
            Capabilities = capabilities,
            Lease = lease,
            Url = url ?? string.Empty,
            NodeStatus = nodeStatus
        };

        if (await _nodeDirectory.CompareAndSet(newNodeId, null, info))
        {
            await Tick();
        }

        _logger.LogInformation($"Joined cluster as ({info.Id})");
        return info;
    }

    public async Task<NodeInfo?> RenewLease(NodeId nodeId, string challengeToken,
        NodeCapabilities capabilities)
    {
        var it = await UpdateNode(nodeId, initialValue =>
        {
            if (initialValue == null || _clock.InPast(initialValue.Lease.ExpiresAt.ToDateTime()))
            {
                throw new InvalidNodeId(nodeId);
            }

            if (initialValue.Lease.ChallengeToken != challengeToken)
            {
                throw new InvalidChallengeException(nodeId, challengeToken);
            }

            var newValue = initialValue;

            newValue.Capabilities = capabilities;
            newValue.Lease = new NodeLease
            {
                ChallengeToken = initialValue.Lease.ChallengeToken,
                ExpiresAt = DateTime.Now.Add(_leaseExpiration.ExpiresIn).ToTimestamp(),
                RenewAt = DateTime.Now.Add(_leaseExpiration.RenewIn).ToTimestamp()
            };
            newValue.VisibleNodes =
                initialValue.VisibleNodes.Intersect(_clusterNodes.Keys.ToList()).ToHashSet();


            return newValue;
        });
        await Tick();
        _logger.LogDebug($"RenewLease {it.Id} in cluster.");
        return it;
    }

    public async Task<NodeInfo?> UpdateNode(NodeId nodeId, Func<NodeInfo?, NodeInfo?> body)
    {
        var it = await _nodeDirectory.Manipulate(nodeId, body);
        await Tick();
        return it;
    }

    public async Task<NodeInfo?> GetNode(NodeId nodeId, bool forceRefresh = false)
    {
        if (forceRefresh)
        {
            return await _nodeDirectory.Get(nodeId);
        }

        try
        {
            var it = await _nodeDirectory.Get(nodeId);
            return _clusterNodes.GetOrAdd(nodeId, _ => it);
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }

    public List<NodeId> FindRoute(NodeId sourceNode, NodeId targetNode)
    {
        if (_nodeGraph.Get() == null)
        {
            BuildGraph();
        }

        var graph = _nodeGraph.Get();

        if (!graph.ContainsVertex(sourceNode))
        {
            _logger.LogDebug($"Source node {sourceNode} not found in cluster.");
            return new List<NodeId>();
        }

        if (!graph.ContainsVertex(targetNode))
        {
            _logger.LogDebug($"Target node {targetNode} not found in cluster.");
            return new List<NodeId>();
        }

        try
        {
            var path = graph.FindPathBetween(sourceNode, targetNode);
            return path?.Skip(1).ToList();
        }
        catch (Exception e)
        {
            _logger.LogDebug($"Could not find path between source and target nodes. {e}");
            return new List<NodeId>();
        }
    }

    private DefaultDirectedGraph<NodeId> BuildGraph()
    {
        var graph = new DefaultDirectedGraph<NodeId>();

        var nodes = _clusterNodes.Values;
        foreach (var node in nodes)
        {
            graph.AddVertex(node.Id);
        }

        graph.BuildVertex();
        foreach (var node in nodes)
        foreach (var visibleNode in node.VisibleNodes)
        {
            if (graph.ContainsVertex(visibleNode))
            {
                graph.AddEdge(node.Id, visibleNode);
            }
        }

        _nodeGraph.AtomicSet(g => graph);
        return graph;
    }
}