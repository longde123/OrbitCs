using Orbit.Server.Net;
using Orbit.Shared.Net;

namespace Orbit.Server.Pipeline.Step;

public class TransportStep : PipelineStep
{
    private readonly ConnectionManager _connectionManager;
    private readonly RemoteMeshNodeManager _remoteMeshNodeManager;

    public TransportStep(ConnectionManager connectionManager, RemoteMeshNodeManager remoteMeshNodeManager)
    {
        this._connectionManager = connectionManager;
        this._remoteMeshNodeManager = remoteMeshNodeManager;
    }

    public override async Task OnOutbound(PipelineContext context, Message msg)
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
    }
}