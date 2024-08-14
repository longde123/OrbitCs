using System.Collections.Concurrent;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Orbit.Server.Auth;
using Orbit.Server.Mesh;
using Orbit.Server.Service;
using Orbit.Shared.Mesh;
using Orbit.Shared.OrException;
using Orbit.Shared.Proto;
using Orbit.Util.Di;
using Mu = Microsoft.Extensions.Logging;

namespace Orbit.Server.Net;

public class ConnectionManager
{
    private readonly AuthSystem _authSystem;
    private readonly ClusterManager _clusterManager;
    private readonly ConcurrentDictionary<NodeId, ClientConnection> _connectedClients;
    private readonly LocalNodeInfo _localNodeInfo;
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly Lazy<Pipeline.Pipeline> _pipeline;

    public ConnectionManager(ClusterManager clusterManager, LocalNodeInfo localNodeInfo,
        AuthSystem authSystem, ComponentContainer container, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ConnectionManager>();
        _loggerFactory = loggerFactory;
        _clusterManager = clusterManager;
        _localNodeInfo = localNodeInfo;
        _authSystem = authSystem;
        _connectedClients = new ConcurrentDictionary<NodeId, ClientConnection>();
        _pipeline = container.Inject<Pipeline.Pipeline>();
        Meters.Gauge(Meters.Names.ConnectedClients, () =>
        {
            _logger.LogDebug($"Gauge ConnectedClients {_connectedClients.Count}");

            return _connectedClients.Count;
        });
    }

    public List<ClientConnection> GetAllConnections()
    {
        return _connectedClients.Values.ToList();
    }

    public ClientConnection? GetClient(NodeId nodeId)
    {
        if (_connectedClients.TryGetValue(nodeId, out var client))
        {
            return client;
        }

        return null;
    }

    public async Task OnNewClient(NodeId nodeId, IAsyncStreamReader<MessageProto> incomingChannel,
        IServerStreamWriter<MessageProto> outgoingChannel, CancellationToken cancellationToken)
    {
        _logger.LogError($"OnNewClient  {nodeId} ");
        NodeInfo? nodeInfo = null;
        //todo
        // try
        // {
        nodeInfo = await _clusterManager.GetNode(nodeId);
        if (nodeInfo == null)
        {
            throw new InvalidNodeId(nodeId);
        }

        var authInfo = await _authSystem.Auth(nodeId) ?? throw new AuthFailed($"Auth failed for {nodeId}");

        var clientConnection =
            new ClientConnection(authInfo, incomingChannel, outgoingChannel, _pipeline.Value, _loggerFactory);
        _connectedClients.TryAdd(nodeId, clientConnection);

        await _clusterManager.UpdateNode(nodeInfo.Id, it =>
        {
            if (it == null)
            {
                throw new Exception($"The node '{nodeInfo.Id}' could not be found in directory on new client.");
            }

            it.VisibleNodes.Add(_localNodeInfo.Info.Id);
            return it;
        });

        await UpdateDirectoryClients();

        _logger.LogInformation($"Client {nodeId} connected to Mesh Node {_localNodeInfo.Info.Id}");
        _logger.LogDebug($"{_localNodeInfo.Info.Id} -> {string.Join(", ", _connectedClients.Keys)}");

        await clientConnection.ConsumeMessages(cancellationToken);
        // }
        // catch (Exception t)
        // {
        //     await outgoingChannel.WriteAsync(new Message(){Content = t.ToErrorContent()}.ToMessageProto());
        // }
        // finally
        {
            if (nodeInfo != null)
            {
                await RemoveNodesFromDirectory(nodeInfo);
            }

            _connectedClients.TryRemove(nodeId, out var _);

            _logger.LogInformation($"Client {nodeId} disconnected from Mesh Node {_localNodeInfo.Info.Id}");
            _logger.LogDebug($"{_localNodeInfo.Info.Id} -> {string.Join(", ", _connectedClients.Keys)}");
        }
        //
    }

    private async Task UpdateDirectoryClients()
    {
        var visibleNodes = _localNodeInfo.Info.VisibleNodes
            .Where(node => node.Namespace == LocalNodeInfo.ManagementNamespace)
            .Concat(_connectedClients.Values.Select(n => n.NodeId))
            .ToHashSet();

        await _localNodeInfo.UpdateInfo(info =>
        {
            info.VisibleNodes = visibleNodes;
            return info;
        });
    }

    private async Task RemoveNodesFromDirectory(NodeInfo nodeInfo)
    {
        await _clusterManager.UpdateNode(nodeInfo.Id, it =>
        {
            if (it == null)
            {
                return null;
            }

            it.VisibleNodes.Remove(_localNodeInfo.Info.Id);
            return it;
        });

        await _localNodeInfo.UpdateInfo(info =>
        {
            info.VisibleNodes.Remove(nodeInfo.Id);
            return info;
        });
    }
}