using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Orbit.Server.Auth;
using Orbit.Server.Mesh;
using Orbit.Server.Net;
using Orbit.Shared.Net;
using Orbit.Shared.OrException;
using Orbit.Util.Concurrent;

namespace Orbit.Server.Pipeline;

public class Pipeline
{
    private readonly OrbitServerConfig _config;
    private readonly LocalNodeInfo _localNodeInfo;
    private readonly ILogger _logger;

    private readonly RailWorker<MessageContainer> _pipelineRails;
    private readonly PipelineSteps _pipelineSteps;

    public Pipeline(OrbitServerConfig config, PipelineSteps pipelineSteps,
        LocalNodeInfo localNodeInfo, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Pipeline>();

        _config = config;
        _pipelineSteps = pipelineSteps;
        _localNodeInfo = localNodeInfo;
        _pipelineRails = new RailWorker<MessageContainer>(
            config.PipelineBufferCount,
            config.PipelineRailCount,
            _logger,
            false,
            OnMessage
        );
    }

    private MessageMetadata LocalMeta => new()
    {
        MessageDirection = MessageDirection.Outbound,
        AuthInfo = new AuthInfo(true, _localNodeInfo.Info.Id),
        RespondOnError = true
    };

    public void Start()
    {
        _logger.LogTrace("Start!");
        _pipelineRails.StartWorkers();
    }

    public async Task Stop()
    {
        _logger.LogTrace("Stop!");
        await _pipelineRails.StopWorkers();
    }

    public async Task PushMessage(Message msg, MessageMetadata? meta = null)
    {
        // if (!_pipelineRails.IsInitialized)
        // {
        //     throw new InvalidOperationException(
        //         "The Orbit pipeline is not in a state to receive messages. Did you start the Orbit server?");
        // }

        var container = new MessageContainer(
            msg,
            meta ?? LocalMeta
        );
        //await OnMessage(container);
        // logger.LogTrace("Writing message to pipeline channel: {0}  !", container);
        //todo
        // try
        // {
        if (!_pipelineRails.Offer(container))
        {
            var errMsg = $"The Orbit pipeline channel is full. >{_config.PipelineBufferCount} buffered messages.";
            _logger.LogError(errMsg);
            throw new CapacityExceededException(errMsg);
        }
        // }
        // catch (CapacityExceededException e)
        // {
        //     throw e;
        // }
        // catch (Exception t)
        // {
        //     throw new Exception("Error offering to pipeline", t);
        // }
    }

    // private async Task LaunchRail(Channel<MessageContainer> receiveChannel)
    // {
    //     await Task.Run(async () =>
    //     {
    //         await foreach (var msg in receiveChannel.Reader.ReadAllAsync())
    //         {
    //             _logger.LogTrace("Pipeline rail received message: {0}", msg);
    //             await OnMessage(msg);
    //         }
    //     });
    // }

    private async Task OnMessage(MessageContainer container)
    {
        var context = new PipelineContext(
            _pipelineSteps.StepIns,
            _pipelineSteps.StepOuts,
            this,
            container.Metadata
        );
        //todo
        try
        {
            await context.Next(container.Message);
            // }
            // catch (PipelineException t)
            // {
            //     logger.LogDebug(t, "Pipeline Error");
            //     if (container.Metadata.RespondOnError)
            //     {
            //         var src = t.LastMsgState.Source ?? container.Metadata.AuthInfo.NodeId;
            //
            //         var newMessage = new Message()
            //         {
            //             MessageId = container.Message.MessageId,
            //             Target = new MessageTarget.Unicast(src),
            //             Content = t.Reason.ToErrorContent()
            //         } ;
            //
            //         var newMeta = LocalMeta;
            //         newMeta.RespondOnError = false;
            //
            //         PushMessage(newMessage, newMeta);
            //     }
            //     else
            //     {
            //         throw t.Reason;
            //     }
        }
        catch (Exception c)
        {
            _logger.LogError(c.ToString());
            throw c;
        }
    }
}