using Orbit.Server.Mesh;
using Orbit.Shared.Mesh;
using Orbit.Shared.Net;

namespace Orbit.Server.Pipeline.Step;

public class PlacementStepIn : PipelineStepIn
{
    private readonly AddressableManager _addressableManager;
    private readonly LocalNodeInfo _localNodeInfo;

    public PlacementStepIn(AddressableManager addressableManager, LocalNodeInfo localNodeInfo)
    {
        _addressableManager = addressableManager;
        _localNodeInfo = localNodeInfo;
    }

    public override async Task<bool> OnInbound(PipelineContext context, Message msg)
    {
        switch (msg.Content)
        {
            case MessageContent.InvocationRequest content:
            {
                var ineligibleNodes = content.Reason == InvocationReason.Rerouted
                    ? new List<NodeId>
                    {
                        context.Metadata.AuthInfo.NodeId
                    }
                    : new List<NodeId>();

                var location =
                    await _addressableManager.LocateOrPlace(msg.Source.Namespace, content.Destination, ineligibleNodes);
                var newMsg = msg;
                newMsg.Target = new MessageTarget.Unicast(location);
                return true;
                break;
            }

            case MessageContent.ConnectionInfoRequest _:
            {
                var source = msg.Source;
                var newMsg = msg;

                newMsg.Target = new MessageTarget.Unicast(source);
                newMsg.Content = new MessageContent.ConnectionInfoResponse(_localNodeInfo.Info.Id);

                return false;
                break;
            }
        }

        return true;
    }
}