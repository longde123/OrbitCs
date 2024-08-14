using Orbit.Server.Net;
using Orbit.Server.Pipeline.Step;
using Orbit.Shared.Net;

namespace Orbit.Server.Pipeline;

public class PipelineContext
{
    private readonly Pipeline _pipeline;
    private readonly PipelineStepIn[] _pipelineStepIns;
    private readonly PipelineStepOut[] _pipelineStepOuts;
    private MessageMetadata _metadata;


    public PipelineContext()
    {
    }

    public PipelineContext(PipelineStepIn[] pipelineSteps, PipelineStepOut[] pipelineStepOuts, Pipeline pipeline, MessageMetadata metadata)
    {
        _pipelineStepIns = pipelineSteps;
        _pipelineStepOuts = pipelineStepOuts;
        _pipeline = pipeline;
        _metadata = metadata;
    }

    public MessageMetadata Metadata => _metadata;

    public async Task Next(Message msg)
    {
        //todo
        // try
        // {
        // if (_metadata.MessageDirection == MessageDirection.Inbound)
        {
            foreach (var pipelineStep in _pipelineStepIns)
            {
                if (!await pipelineStep.OnInbound(this, msg))
                {
                    break;
                }
            }
        }
        // else if (_metadata.MessageDirection == MessageDirection.Outbound)
        {
            foreach (var pipelineStep in _pipelineStepOuts)
            {
                if (!await pipelineStep.OnOutbound(this, msg))
                {
                    break;
                }
            }
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

    public void PushNew(Message msg)
    {
        //   _pipeline?.PushMessage(msg, null);
    }
}