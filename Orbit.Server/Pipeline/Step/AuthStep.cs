using Orbit.Shared.Net;

namespace Orbit.Server.Pipeline.Step;

public class AuthStep : PipelineStep
{
    public override async Task OnInbound(PipelineContext context, Message msg)
    {
        var newSource = msg.Source != null && context.Metadata.AuthInfo.IsManagementNode
            ? msg.Source
            : context.Metadata.AuthInfo.NodeId;
        //todo
        msg.Source = newSource;
        var newMsg = msg;

        await context.Next(newMsg);
    }
}