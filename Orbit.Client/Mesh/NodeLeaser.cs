using Microsoft.Extensions.Logging;
using Orbit.Client.Net;
using Orbit.Shared.Proto;
using Orbit.Util.Time;

namespace Orbit.Client.Mesh;

public class NodeLeaser
{
    //租约 (Leasing).

    private readonly Clock _clock;
    private readonly OrbitClientConfig _config;
    private readonly GrpcClient _grpcClient;
    private readonly TimeSpan _joinTimeout;
    private readonly TimeSpan _leaveTimeout;
    private readonly LocalNode _localNode;
    private readonly ILogger _logger;
    private readonly NodeManagement.NodeManagementClient _nodeManagementStub;

    public NodeLeaser(LocalNode localNode, GrpcClient grpcClient, OrbitClientConfig config, Clock clock,
        ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<NodeLeaser>();
        _localNode = localNode;
        _grpcClient = grpcClient;
        _config = config;
        _clock = clock;
        _joinTimeout = config.JoinClusterTimeout;
        _leaveTimeout = config.LeaveClusterTimeout;
        _nodeManagementStub = new NodeManagement.NodeManagementClient(grpcClient.Channel);
    }

    public async Task JoinCluster()
    {
        _logger.LogInformation(
            $"Joining namespace '{_localNode.Status.Namespace}' in the '{_localNode.Status.GrpcEndpoint}' cluster ...");
        var deadline = DateTime.Now.ToUniversalTime() + _joinTimeout;
        var joinClusterRequestProto = new JoinClusterRequestProto
        {
            Capabilities = _localNode.Status.Capabilities?.ToCapabilitiesProto()
        };
        var responseProto = await _nodeManagementStub.JoinClusterAsync(joinClusterRequestProto, null, deadline);

        if (responseProto.Status != NodeLeaseResponseProto.Types.Status.Ok)
        {
            throw new NodeLeaseRenewalFailed("Joining cluster failed");
        }

        var nodeInfo = responseProto.Info.ToNodeInfo();
        _localNode.Manipulate(node =>
        {
            node.NodeInfo = nodeInfo;
            return node;
        });
        _logger.LogInformation($"Joined cluster as node '{nodeInfo.Id}'.");
    }

    public async Task RenewLease(bool force)
    {
        if (_localNode.Status.NodeInfo != null)
        {
            var existingInfo = _localNode.Status.NodeInfo;
            var existingLease = existingInfo.Lease;

            if (force || _clock.InPast(existingLease.RenewAt.ToDateTime()))
            {
                _logger.LogDebug("Renewing lease...");
                var renewalResult = await _nodeManagementStub.RenewLeaseAsync(new RenewNodeLeaseRequestProto
                {
                    ChallengeToken = existingLease.ChallengeToken,
                    Capabilities = _localNode.Status.Capabilities?.ToCapabilitiesProto()
                });

                if (renewalResult.Status != NodeLeaseResponseProto.Types.Status.Ok)
                {
                    throw new NodeLeaseRenewalFailed("Node renewal failed");
                }

                _localNode.Manipulate(node =>
                {
                    node.NodeInfo = renewalResult.Info.ToNodeInfo();
                    return node;
                });
                _logger.LogDebug("Lease renewed.");
            }
        }
    }

    public async Task LeaveCluster()
    {
        _logger.LogInformation($"Leaving namespace '{_localNode.Status.Namespace}' cluster ...");
        var deadline = DateTime.Now.ToUniversalTime() + _leaveTimeout;
        var responseProto =
            await _nodeManagementStub.LeaveClusterAsync(new LeaveClusterRequestProto(), null,
                deadline);

        var nodeInfo = responseProto.Info.ToNodeInfo();
        _localNode.Manipulate(node =>
        {
            node.NodeInfo = nodeInfo;
            return node;
        });
        _logger.LogInformation("Left cluster");
    }

    public async Task Tick()
    {
        _logger.LogWarning("Tick");
        await RenewLease(false);
    }
}