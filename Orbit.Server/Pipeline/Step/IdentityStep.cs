using Orbit.Server.Mesh;
using Orbit.Shared.Net;
using Orbit.Util.Concurrent;

namespace Orbit.Server.Pipeline.Step;

public class IdentityStep : PipelineStep
{
    private readonly LocalNodeInfo _localNodeInfo;
    private readonly AtomicReference<long> _messageCounter = new(0);

    public IdentityStep(LocalNodeInfo localNodeInfo)
    {
        this._localNodeInfo = localNodeInfo;
    }

    public override async Task OnOutbound(PipelineContext context, Message msg)
    {
        var newSource = msg;
        newSource.MessageId = msg.MessageId ?? _messageCounter.AtomicSet(a => _messageCounter.Get() + 1);
        newSource.Source = msg.Source ?? _localNodeInfo.Info.Id;

        await context.Next(newSource);
    }
}