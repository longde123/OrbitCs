using Orbit.Server.Service;
using Orbit.Shared.Net;

namespace Orbit.Server.Pipeline.Step;

public class RoutingStepIn : PipelineStepIn
{
    public override async Task<bool> OnInbound(PipelineContext context, Message msg)
    {
        if (msg.Target == null)
        {
            throw new Exception("Node target was not resolved");
        }

        return false;
    }
}