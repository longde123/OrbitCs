using Orbit.Server.Mesh;
using Orbit.Shared.Net;
using Orbit.Util.Concurrent;

namespace Orbit.Server.Pipeline.Step;

public class IdentityStepOut : PipelineStepOut
{
    private readonly LocalNodeInfo _localNodeInfo;
    private readonly AtomicReference<long> _messageCounter = new(0);

    public IdentityStepOut(LocalNodeInfo localNodeInfo)
    {
        _localNodeInfo = localNodeInfo;
    }

    public override async Task<bool> OnOutbound(PipelineContext context, Message msg)
    {
        var newSource = msg;
        newSource.MessageId = msg.MessageId ?? _messageCounter.AtomicSet(a => _messageCounter.Get() + 1);
        newSource.Source = msg.Source ?? _localNodeInfo.Info.Id;
        return true;
    }
}