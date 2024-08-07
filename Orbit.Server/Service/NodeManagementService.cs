using Grpc.Core;
using Microsoft.Extensions.Logging;
using Orbit.Server.Concurrent;
using Orbit.Server.Mesh;
using Orbit.Shared.Mesh;
using Orbit.Shared.Proto;

namespace Orbit.Server.Service;

public class NodeManagementService : NodeManagement.NodeManagementBase
{
    private readonly ClusterManager _clusterManager;
    private readonly ILogger _logger;
    private RuntimeScopes _runtimeScopes;

    public NodeManagementService(ClusterManager clusterManager, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<NodeManagementService>();
        this._clusterManager = clusterManager;
    }

    public override async Task<NodeLeaseResponseProto> JoinCluster(JoinClusterRequestProto request,
        ServerCallContext context)
    {
        try
        {
            var nameSpace = (string)context.UserState[ServerAuthInterceptor.Namespace];
            var capabilities = request.Capabilities.ToCapabilities();
            var info = await _clusterManager.JoinCluster(nameSpace, capabilities, null, NodeStatus.Active);
            _logger.LogDebug($"Joining cluster {info.Id}");
            return info.ToNodeLeaseRequestResponseProto();
        }
        catch (Exception t)
        {
            return t.ToNodeLeaseRequestResponseProto();
        }
    }

    public override async Task<NodeLeaseResponseProto> RenewLease(RenewNodeLeaseRequestProto request,
        ServerCallContext context)
    {
        //todo
        try
        {
            var nodeId = (NodeId)context.UserState[ServerAuthInterceptor.NodeId];
            if (nodeId == null)
            {
                throw new Exception("Node ID was not specified");
            }

            var capabilities = request.Capabilities.ToCapabilities();
            var challengeToken = request.ChallengeToken;
            var info = await _clusterManager.RenewLease(nodeId, challengeToken, capabilities);
            var it = info.ToNodeLeaseRequestResponseProto();
            return it;
        }
        catch (Exception t)
        {
            return t.ToNodeLeaseRequestResponseProto();
        }
    }

    public override async Task<NodeLeaseResponseProto> LeaveCluster(LeaveClusterRequestProto request,
        ServerCallContext context)
    {
        var nodeId = (NodeId)context.UserState[ServerAuthInterceptor.NodeId];
        if (nodeId == null)
        {
            throw new Exception("Node ID was not specified");
        }

        var nodeInfo = await _clusterManager.UpdateNode(nodeId, it =>
        {
            _logger.LogDebug($"The node '{nodeId}' was not found in directory while leaving the cluster.");
            it.NodeStatus = NodeStatus.Draining;

            return it;
        });

        var nodeLeaseResponseProto = new NodeLeaseResponseProto
        {
            Info = nodeInfo.ToNodeInfoProto()
        };
        return nodeLeaseResponseProto;
    }
}