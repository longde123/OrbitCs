using System.Collections.Concurrent;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Orbit.Shared.Net;
using Orbit.Shared.Proto;
using Orbit.Util.Concurrent;
using Orbit.Util.Di;

namespace Orbit.Client.Net;

public class ConnectionHandler
{
    private readonly ILogger _logger;

    private readonly Lazy<MessageHandler> _messageHandler;

    //private readonly RailWorker<Message> _messageRails;
    private readonly Connection.ConnectionClient _messagesStub;
    private readonly BlockingCollection<Message> _sendDataQueue;
    private AsyncDuplexStreamingCall<MessageProto, MessageProto>? _connectionChannel;
    private GrpcClient _grpcClient;
    private CancellationTokenSource _tokenSource;

    public ConnectionHandler(OrbitClientConfig config, GrpcClient grpcClient,
        ComponentContainer componentContainer, ILoggerFactory loggerFactory)
    {
        _grpcClient = grpcClient;
        _logger = loggerFactory.CreateLogger<ConnectionHandler>();
        _messagesStub = new Connection.ConnectionClient(grpcClient.Channel);
        _messageHandler = componentContainer.Inject<MessageHandler>();
        //  _messageRails = new RailWorker<Message>(config.BufferCount, config.RailCount, _logger, false, OnMessage);
        _sendDataQueue = new BlockingCollection<Message>();
    }

    public void Connect()
    {
        //   _messageRails.StartWorkers();
        _tokenSource = new CancellationTokenSource();
        var callOptions = new CallOptions(cancellationToken: _tokenSource.Token);
        _connectionChannel = _messagesStub.OpenStream(callOptions);

        Task.Run(async () =>
        {
            while (await _connectionChannel.ResponseStream.MoveNext())
                // await foreach (var msg in _connectionChannel.ResponseStream.ReadAllAsync())
            {
                var msg = _connectionChannel.ResponseStream.Current;
                _logger.LogDebug($"Received message {msg.MessageId} to {msg.Target}");
                //   _messageRails.Offer(msg.ToMessage());
                OnMessage(msg.ToMessage());
            }
        }, _tokenSource.Token);

        Task.Run(async () =>
        {
            foreach (var msg in _sendDataQueue.GetConsumingEnumerable())
            {
                _logger.LogDebug($"Send {msg.Destination}=>{msg.MessageId}");
                var toMessageProto = msg.ToMessageProto();
                if (!_tokenSource.IsCancellationRequested)
                {
                    await _connectionChannel.RequestStream.WriteAsync(toMessageProto);
                }
            }
        }, _tokenSource.Token);
    }

    public void Tick()
    {
        _logger.LogWarning("Tick");
        TestConnection();
    }

    public async Task Disconnect()
    {
        if (_connectionChannel != null)
        {
            _tokenSource.Cancel();
            _connectionChannel.Dispose();
            _connectionChannel = null;
            // await _messageRails.StopWorkers();
        }
    }

    public void Send(Message msg)
    {
        TestConnection();
        {
            _sendDataQueue.Add(msg);
        }
    }

    private async Task OnMessage(Message message)
    {
        if (!_tokenSource.IsCancellationRequested)
        {
            await _messageHandler.Value.OnMessage(message);
        }
    }

    private async Task TestConnection()
    {
        if (_connectionChannel != null)
        {
            if (_connectionChannel.GetStatus().StatusCode != StatusCode.OK)
            {
                _logger.LogWarning("The stream connection is closed. Reopening...");
                await Disconnect();
                Connect();
            }
        }
        else
        {
            _logger.LogDebug("Testing connection but is not initialized");
        }
    }
}