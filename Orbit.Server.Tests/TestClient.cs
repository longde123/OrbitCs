/*
Converted from Kotlin to C#
*/

using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Orbit.Shared.Addressable;
using Orbit.Shared.Mesh;
using Orbit.Shared.Net;
using Orbit.Shared.Proto;

namespace Orbit.Server;

public class TestClient
{
    private readonly Action<Message> _onReceive;
    private AddressableManagement.AddressableManagementClient _addressableChannel;

    private string _challengeToken;
    private GrpcChannel _channel;

    private AsyncDuplexStreamingCall<MessageProto, MessageProto> _connectionChannel;
    private long _messageId;
    private NodeManagement.NodeManagementClient _nodeChannel;

    public TestClient(Action<Message> onReceive)
    {
        _onReceive = onReceive;
    }

    public NodeId NodeId { get; private set; } = NodeId.Generate("test");

    public async Task<TestClient> Connect(int port = 50056)
    {
        var httpHandler = new HttpClientHandler();
        httpHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        _channel = GrpcChannel.ForAddress($"https://localhost:{port}", new GrpcChannelOptions
        {
            HttpClient = null,
            HttpHandler = httpHandler
        });
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Trace); // 设置最低日志级别
        });
        var invoker = _channel.Intercept(new TestAuthInterceptor(() => NodeId, loggerFactory));
        _nodeChannel = new NodeManagement.NodeManagementClient(invoker);

        var connectionClient = new Connection.ConnectionClient(invoker);
        _addressableChannel = new AddressableManagement.AddressableManagementClient(invoker);

        var response = await _nodeChannel.JoinClusterAsync(new JoinClusterRequestProto
        {
            Capabilities = new CapabilitiesProto
            {
                AddressableTypes =
                {
                    "test"
                }
            }
        });
        NodeId = response.Info.Id.ToNodeId();
        _challengeToken = response.Info.Lease.ChallengeToken;
        _connectionChannel = connectionClient.OpenStream();


        _ = Task.Run(async () =>
        {
            await foreach (var msg in _connectionChannel.ResponseStream.ReadAllAsync())
            {
                OnMessage(msg.ToMessage());
            }
        });


        return this;
    }

    public async Task Disconnect()
    {
        await _connectionChannel.RequestStream.CompleteAsync();
        _connectionChannel.Dispose();
        await _channel.ShutdownAsync();
    }

    public async Task Drain()
    {
        await _nodeChannel.LeaveClusterAsync(new LeaveClusterRequestProto());
    }

    public void OnMessage(Message msg)
    {
        var content = msg.Content is MessageContent.InvocationRequest
            ? (msg.Content as MessageContent.InvocationRequest).Arguments
            : $"Error: {(msg.Content as MessageContent.Error).Description}";
        Console.WriteLine($"Message received on node {NodeId} - {content}");
        _onReceive(msg);
    }

    public async Task SendMessage(string msg, string address = null)
    {
        Console.WriteLine($"Sending message to {address} - {msg}");

        var addressableReference =
            new AddressableReference("test", address != null ? new Key.StringKey(address) : Key.None());

        var invocationRequest = new MessageContent.InvocationRequest(addressableReference,
            "report",
            msg
        );
        var message = new Message
        {
            Content = invocationRequest,
            Source = NodeId,
            MessageId = ++_messageId
        };
        await _connectionChannel.RequestStream.WriteAsync(message.ToMessageProto());
        // await connectionChannel.RequestStream.CompleteAsync();
    }

    public async Task<AddressableLease> RenewAddressableLease(string address)
    {
        var response = await _addressableChannel.RenewLeaseAsync(new RenewAddressableLeaseRequestProto
        {
            Reference = new AddressableReference("test", Key.Of(address)).ToAddressableReferenceProto()
        });

        return response.Lease?.ToAddressableLease();
    }

    public async Task<NodeInfo> RenewNodeLease()
    {
        var response = await _nodeChannel.RenewLeaseAsync(new RenewNodeLeaseRequestProto
        {
            Capabilities = new CapabilitiesProto
            {
                AddressableTypes =
                {
                    "test"
                }
            },
            ChallengeToken = _challengeToken
        });

        return response.Info?.ToNodeInfo();
    }
}