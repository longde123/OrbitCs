using System.Collections.Concurrent;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Orbit.Server.Mesh;
using Orbit.Server.Service;
using Orbit.Shared.Mesh;
using Orbit.Shared.Net;
using Orbit.Shared.Proto;
using Orbit.Shared.Router;

namespace Orbit.Server.Net;

public class RemoteMeshNodeConnection : IMessageSender
{
    private readonly GrpcChannel _channel;
    public readonly NodeId Id;
    private readonly LocalNodeInfo _localNode;
    private readonly ILogger _logger;
    private readonly BlockingCollection<Message> _sendDataQueue;
    private readonly AsyncDuplexStreamingCall<MessageProto, MessageProto> _sender;
    private CancellationTokenSource _tokenSource;

    public RemoteMeshNodeConnection(LocalNodeInfo localNode, NodeInfo remoteNode, ILoggerFactory loggerFactory)
    {
        _localNode = localNode;
        Id = remoteNode.Id;
        var httpHandler = new HttpClientHandler();
        httpHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        _logger = loggerFactory.CreateLogger<RemoteMeshNodeConnection>();
        var channelHttps = GrpcChannel.ForAddress(remoteNode.Url, new GrpcChannelOptions
        {
            HttpClient = null,
            HttpHandler = httpHandler
        });
        _channel = channelHttps;
        var invoker = _channel.Intercept(new ClientAuthInterceptor(localNode, loggerFactory));
        _sender = new Connection.ConnectionClient(invoker).OpenStream();

        _sendDataQueue = new BlockingCollection<Message>();
        Notify(_channel);
    }

    public async Task SendMessage(Message message, Route? route=null)
    {
        _sendDataQueue.Add(message);
    }

    private async Task Notify(GrpcChannel channel)
    {
        // while (  channel.State != ConnectivityState.Shutdown)
        // {
        //     
        // }
        _tokenSource = new CancellationTokenSource();
        Task.Run(async () =>
        {
            foreach (var message in _sendDataQueue.GetConsumingEnumerable())
            {
                try
                {
                    _sender.RequestStream.WriteAsync(message.ToMessageProto());
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error {e}");
                }
            }
        }, _tokenSource.Token);
    }

    public async Task Disconnect()
    {
        _tokenSource.Cancel();
        _sender.Dispose();
        await _channel.ShutdownAsync();
    }
}