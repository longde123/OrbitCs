using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Orbit.Server.Mesh;
using Orbit.Server.Service;
using Orbit.Shared.Mesh;

namespace Orbit.Server.Net;

public class RemoteMeshNodeManager
{
    private readonly ClusterManager _clusterManager;
    private readonly ConcurrentDictionary<NodeId, RemoteMeshNodeConnection> _connections;
    private readonly LocalNodeInfo _localNode;
    private readonly ILogger<RemoteMeshNodeManager> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public RemoteMeshNodeManager(LocalNodeInfo localNode, ClusterManager clusterManager, ILoggerFactory loggerFactory)
    {
        this._localNode = localNode;
        this._clusterManager = clusterManager;
        this._loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<RemoteMeshNodeManager>();
        _connections = new ConcurrentDictionary<NodeId, RemoteMeshNodeConnection>();

        Meters.Gauge(Meters.Names.ConnectedNodes, () =>
        {
            _logger.LogDebug($"ConnectedNodes {_connections.Count}.");
            return _connections.Count;
        });
    }

    public async Task Tick()
    {
        await RefreshConnections();
    }
    public List<RemoteMeshNodeConnection> GetAllConnections()
    {
        return _connections.Values.ToList();
    }
    public RemoteMeshNodeConnection? GetNode(NodeId nodeId)
    {
        _connections.TryGetValue(nodeId, out var connection);
        return connection;
    }

    public async Task RefreshConnections()
    {
        var allNodes = _clusterManager.GetAllNodes();

        var meshNodes = allNodes
            .Where(node => node.NodeStatus == NodeStatus.Active)
            .Where(node => node.Id.Namespace == LocalNodeInfo.ManagementNamespace)
            .Where(node => !_connections.ContainsKey(node.Id))
            .Where(node => !node.Id.Equals(_localNode.Info.Id))
            .Where(node => node.Url != null && node.Url != _localNode.Info.Url)
            .ToList();

        foreach (var node in meshNodes)
        {
            _logger.LogInformation($"Connecting to peer {node.Id.Key} @{node.Url}...");
            _connections[node.Id] = new RemoteMeshNodeConnection(_localNode, node, _loggerFactory);
            _logger.LogDebug($"{_localNode.Info.Id} -> {string.Join(", ", _connections.Keys)}");
        }

        foreach (var node in _connections.Values)
        {
            if (!allNodes.Any(n => n.Id.Equals(node.Id)))
            {
                _logger.LogInformation($"Removing peer {node.Id.Key}...");
                await _connections[node.Id].Disconnect();
                _connections.TryRemove(node.Id, out _);
                _logger.LogDebug($"{_localNode.Info.Id} -> {string.Join(", ", _connections.Keys)}");
            }
        }

        var visibleNodes = _localNode.Info.VisibleNodes
            .Where(node => node.Namespace != LocalNodeInfo.ManagementNamespace)
            .Concat(_connections.Values.Select(n => n.Id))
            .ToHashSet();

        if (!visibleNodes.SetEquals(_localNode.Info.VisibleNodes))
        {
            await _clusterManager.UpdateNode(_localNode.Info.Id, node =>
            {
                node.VisibleNodes = visibleNodes;
                return node;
            });
        }
    }
}