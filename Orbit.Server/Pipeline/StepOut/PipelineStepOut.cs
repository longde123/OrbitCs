using Orbit.Shared.Net;

namespace Orbit.Server.Pipeline.Step;

public class PipelineStepOut
{
    public virtual async Task<bool> OnOutbound(PipelineContext context, Message msg)
    {
        return false;
    }
}