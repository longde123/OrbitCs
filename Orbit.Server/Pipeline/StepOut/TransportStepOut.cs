using Orbit.Server.Net;
using Orbit.Shared.Net;

namespace Orbit.Server.Pipeline.Step;

public class TransportStepOut : PipelineStepOut
{
    private readonly ConnectionManager _connectionManager;
    private readonly RemoteMeshNodeManager _remoteMeshNodeManager;

    public TransportStepOut(ConnectionManager connectionManager, RemoteMeshNodeManager remoteMeshNodeManager)
    {
        _connectionManager = connectionManager;
        _remoteMeshNodeManager = remoteMeshNodeManager;
    }

    public override async Task<bool> OnOutbound(PipelineContext context, Message msg)
    {
        var targetNode = msg.Target switch
        {
            MessageTarget.Unicast target => target.TargetNode,
            MessageTarget.RoutedUnicast target => target.Route.NextNode,
            _ => null
        };

        if (targetNode == null)
        {
            throw new Exception($"Could not determine a target {msg.Target}");
        }

        IMessageSender? client = _remoteMeshNodeManager.GetNode(targetNode);
        if (client == null)
        {
            client = _connectionManager.GetClient(targetNode);
        }

        if (client == null)
        {
            throw new Exception($"Could not find target {targetNode} in connections");
        }

        await client?.SendMessage(msg);
        return true;
    }
}