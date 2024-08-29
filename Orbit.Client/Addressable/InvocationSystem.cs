using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Orbit.Client.Execution;
using Orbit.Client.Net;
using Orbit.Client.Reactive;
using Orbit.Client.Util;
using Orbit.Shared.Addressable;
using Orbit.Shared.Net;
using Orbit.Shared.OrException;
using Orbit.Util.Concurrent;
using Orbit.Util.Di;
using AddressableType = System.String;
using AddressableInvocationArgument = System.Tuple<object, System.Type>;
using AddressableInvocationArguments = System.Collections.Generic.List<System.Tuple<object, System.Type>>;

namespace Orbit.Client.Addressable;

public partial class InvocationSystem
{
    private readonly OrbitClientConfig _config;
    private readonly ExecutionSystem _executionSystem;
    private readonly LocalNode _localNode;
    private readonly ILogger _logger;
    private readonly Lazy<MessageHandler> _messageHandler;
    private readonly Serializer.Serializer _serializer;


    public InvocationSystem(Serializer.Serializer serializer, ExecutionSystem executionSystem, LocalNode localNode,
        OrbitClientConfig config, ComponentContainer componentContainer, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<InvocationSystem>();
        _serializer = serializer;
        _executionSystem = executionSystem;
        _localNode = localNode;
        _config = config;
        _messageHandler = componentContainer.Inject<MessageHandler>();
        
    }

    public async Task OnInvocationRequest(Message msg)
    {
        var content = msg.Content as MessageContent.InvocationRequest;

        var arguments = _serializer.Deserialize<AddressableInvocationArguments>(content.Arguments);
        var invocation = new AddressableInvocation
        {
            Reference = content.Destination,
            Method = content.Method,
            Args = arguments
        };

        MessageContent response;
        try
        {
            var completion = new Completion();
            await _executionSystem.HandleInvocation(invocation, completion);
            var result = await completion.Task;
            if (null == result)
            {
                response = new MessageContent.InvocationResponse("null");
            }
            else
            {
                var resultSerialize = _serializer.Serialize(result);
                response = new MessageContent.InvocationResponse(resultSerialize);
            }
        }
        catch (Exception t)
        {
            response = t switch
            {
                RerouteMessageException => new MessageContent.InvocationResponseError(t.ToString(),
                    _serializer.Serialize(t)),
                _ when _config.PlatformExceptions => new MessageContent.InvocationResponseError(t.ToString(),
                    _serializer.Serialize(t)),
                _ => new MessageContent.Error(t.ToString())
            };
        }

        _messageHandler.Value.SendMessage(new Message
        {
            MessageId = msg.MessageId,
            Target = new MessageTarget.Unicast(msg.Source),
            Content = response
        });
        _logger.LogDebug($"SendInvocationResponse {msg.Destination} -> {msg.MessageId} ");
    }

    public void OnInvocationResponse(MessageContent.InvocationResponse ir, Completion completion)
    {
        var result = _serializer.Deserialize<object>(ir.Data);
        completion.TrySetResult(result);
    }

    public void OnInvocationPlatformErrorResponse(MessageContent.InvocationResponseError ire, Completion completion)
    {
        var result = _config.PlatformExceptions && !string.IsNullOrEmpty(ire.Platform)
            ? _serializer.Deserialize<Exception>(ire.Platform)
            : new RemoteException($"Exceptional response received: {ire.Description}");
        //  completion.SetResult(null);
        completion.TrySetException(result);
    }


    public void SendInvocation(AddressableInvocation invocation, Completion completion)
    {
        if (_localNode.Status.ClientState != ClientState.Connected)
        {
            throw new InvalidOperationException("The Orbit client is not connected");
        }

        var arguments = _serializer.Serialize(invocation.Args);
        var msg = new Message
        {
            Content = new MessageContent.InvocationRequest(invocation.Reference, invocation.Method, arguments,
                invocation.Reason)
        };
        _messageHandler.Value.SendMessage(msg, completion);
        _logger.LogDebug($"SendInvocationRequest {msg.Destination} -> {msg.MessageId} ");
    }
}