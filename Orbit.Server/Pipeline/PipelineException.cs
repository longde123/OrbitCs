using Orbit.Shared.Net;

namespace Orbit.Server.Pipeline;

public class PipelineException : Exception
{
    public PipelineException(Message lastMsgState, Exception reason) : base(null, reason)
    {
        LastMsgState = lastMsgState;
        Reason = reason;
    }

    public Message LastMsgState { get; }
    public Exception Reason { get; }
}