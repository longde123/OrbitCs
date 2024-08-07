using Orbit.Server.Net;
using Orbit.Server.Pipeline.Step;
using Orbit.Shared.Net;

namespace Orbit.Server.Pipeline;

public class PipelineContext
{
    private readonly Pipeline _pipeline;
    private readonly int _pipelineSize;
    private readonly PipelineStep[] _pipelineSteps;
    private MessageMetadata _metadata;
    private int _pointer;

    public PipelineContext()
    {
    }

    public PipelineContext(PipelineStep[] pipelineSteps, Pipeline pipeline, MessageMetadata metadata)
    {
        this._pipelineSteps = pipelineSteps;
        this._pipeline = pipeline;
        this._metadata = metadata;

        _pipelineSize = pipelineSteps.Length;
        _pointer = metadata.MessageDirection == MessageDirection.Inbound ? _pipelineSize : -1;
    }

    public MessageMetadata Metadata => _metadata;

    public virtual async Task Next(Message msg)
    {
        //todo
        // try
        // {
        if (_metadata.MessageDirection == MessageDirection.Inbound)
        {
            if (--_pointer < 0)
            {
                throw new Exception("Beginning of pipeline encountered.");
            }

            var pipelineStep = _pipelineSteps[_pointer];
            await pipelineStep.OnInbound(this, msg);
        }
        else if (_metadata.MessageDirection == MessageDirection.Outbound)
        {
            if (++_pointer >= _pipelineSize)
            {
                throw new Exception("End of pipeline encountered.");
            }

            var pipelineStep = _pipelineSteps[_pointer];
            await pipelineStep.OnOutbound(this, msg);
        }
        // }
        // catch (PipelineException t)
        // {
        //     throw t;
        // }
        // catch (Exception t)
        // {
        //     throw new PipelineException(msg, t);
        // }
    }

    public virtual void PushNew(Message msg, MessageMetadata newMeta = null)
    {
        _pipeline?.PushMessage(msg, newMeta);
    }
}