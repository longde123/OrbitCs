using Orbit.Shared.Net;

namespace Orbit.Server.Pipeline.Step;

public class AuthStepIn : PipelineStepIn
{
    public override async Task<bool> OnInbound(PipelineContext context, Message msg)
    {
        var newSource = msg.Source != null && context.Metadata.AuthInfo.IsManagementNode
            ? msg.Source
            : context.Metadata.AuthInfo.NodeId;
        //todo
        msg.Source = newSource;
        var newMsg = msg;
        return true;
    }
}