using Orbit.Server.Mesh;
using Orbit.Shared.Mesh;
using Orbit.Shared.Net;

namespace Orbit.Server.Pipeline.Step;

public class PlacementStep : PipelineStep
{
    private readonly AddressableManager _addressableManager;
    private readonly LocalNodeInfo _localNodeInfo;

    public PlacementStep(AddressableManager addressableManager, LocalNodeInfo localNodeInfo)
    {
        this._addressableManager = addressableManager;
        this._localNodeInfo = localNodeInfo;
    }

    public override async Task OnInbound(PipelineContext context, Message msg)
    {
        switch (msg.Content)
        {
            case MessageContent.InvocationRequest content:
            {
                var ineligibleNodes = content.Reason == InvocationReason.Rerouted
                    ? new List<NodeId> { context.Metadata.AuthInfo.NodeId }
                    : new List<NodeId>();

                var location =
                    await _addressableManager.LocateOrPlace(msg.Source.Namespace, content.Destination, ineligibleNodes);
                var newMsg = msg;
                newMsg.Target = new MessageTarget.Unicast(location);
                await context.Next(newMsg);
                break;
            }
            case MessageContent.ConnectionInfoRequest _:
            {
                var source = msg.Source;
                var newMsg = msg;

                newMsg.Target = new MessageTarget.Unicast(source);
                newMsg.Content = new MessageContent.ConnectionInfoResponse(_localNodeInfo.Info.Id);
                context.PushNew(newMsg);
                break;
            }
             
            default:
                await context.Next(msg);
                break;
        }
    }
}