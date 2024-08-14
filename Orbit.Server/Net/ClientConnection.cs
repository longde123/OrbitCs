using System.Collections.Concurrent;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Orbit.Server.Auth;
using Orbit.Server.Service;
using Orbit.Shared.Mesh;
using Orbit.Shared.Net;
using Orbit.Shared.OrException;
using Orbit.Shared.Proto;
using Orbit.Shared.Router;

namespace Orbit.Server.Net;

public class ClientConnection : IMessageSender
{
    private readonly AuthInfo _authInfo;
    private readonly IAsyncStreamReader<MessageProto> _incomingChannel;
    private readonly ILogger _logger;
    private readonly IServerStreamWriter<MessageProto> _outgoingChannel;
    private readonly Pipeline.Pipeline _pipeline;
    private readonly BlockingCollection<Message> _sendDataQueue;
    private readonly CancellationTokenSource _tokenSource;

    public ClientConnection(AuthInfo authInfo, IAsyncStreamReader<MessageProto> incomingChannel,
        IServerStreamWriter<MessageProto> outgoingChannel, Pipeline.Pipeline pipeline, ILoggerFactory loggerFactory)
    {
        _authInfo = authInfo;
        _incomingChannel = incomingChannel;
        _outgoingChannel = outgoingChannel;
        _pipeline = pipeline;
        _logger = loggerFactory.CreateLogger<ClientConnection>();
        _sendDataQueue = new BlockingCollection<Message>();
        _tokenSource = new CancellationTokenSource();
        Task.Run(async () =>
        {
            foreach (var message in _sendDataQueue.GetConsumingEnumerable())
            {
                try
                {
                    var mp = message.ToMessageProto();
                    outgoingChannel.WriteAsync(mp);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error {e}");
                }
            }
        }, _tokenSource.Token);
    }

    public NodeId NodeId => _authInfo.NodeId;

    public async Task SendMessage(Message message, Route? route = null)
    {
        _logger.LogInformation($"SendMessage {message.Destination}->{message.MessageId} !");
        _sendDataQueue.Add(message);
    }

    public async Task ConsumeMessages(CancellationToken cancellationToken)
    {
        var messageSizes = Meters.Summary(Meters.Names.MessageSizes);

        await foreach (var protoMessage in _incomingChannel.ReadAllAsync())
        {
            if (cancellationToken.IsCancellationRequested)
                // 处理取消逻辑
            {
                break;
            }

            await messageSizes.Record(async () =>
            {
                _logger.LogError($"messageSizes {protoMessage.ToByteArray().Length} ");
                return protoMessage.ToByteArray().Length;
            });

            var message = protoMessage.ToMessage();
            _logger.LogError($"ConsumeMessages {message.Destination}->{message.MessageId} ");
            var meta = new MessageMetadata(_authInfo, MessageDirection.Inbound);
            //todo
            // try  
            // {
            await _pipeline.PushMessage(message, meta);
            // }
            // catch (Exception t)
            // {
            //     pipeline.PushMessage(new Message()
            //     {
            //         Target =  message.Source != null ? new MessageTarget.Unicast(message.Source) : null,
            //         MessageId= message.MessageId,
            //         Content= t.ToErrorContent()
            //     } , new MessageMetadata(authInfo, MessageDirection.OUTBOUND, false));
            // }
        }

        Close();
    }

    public void OfferMessage(Message message)
    {
        try
        {
            _outgoingChannel.WriteAsync(message.ToMessageProto());
        }
        catch (Exception e)
        {
            throw new CapacityExceededException($"Could not offer message. {e}");
        }
    }

    public void Close(Exception? cause = null, long? messageId = null)
    {
        _tokenSource.Cancel();
        if (cause != null)
        {
            OfferMessage(new Message
            {
                MessageId = messageId,
                Content = cause.ToErrorContent()
            });
        }
    }
}