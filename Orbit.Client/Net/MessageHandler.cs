using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Orbit.Client.Addressable;
using Orbit.Client.Reactive;
using Orbit.Client.Util;
using Orbit.Shared.Addressable;
using Orbit.Shared.Net;
using Orbit.Util.Concurrent;
using Orbit.Util.Time;
using TimeoutException = System.TimeoutException;

namespace Orbit.Client.Net;

public class MessageHandler
{
    private readonly ConcurrentDictionary<long, ResponseEntry> _awaitingResponse;
    private readonly ConcurrentDictionary<long, Completion> _awaitingSubject;
    private readonly Clock _clock;
    private readonly OrbitClientConfig _config;
    private readonly ConnectionHandler _connectionHandler;
    private readonly InvocationSystem _invocationSystem;

    private readonly ILogger _logger;
    private readonly AtomicReference<long> _messageCounter;

    public AtomicReference<long> MessageCounter => _messageCounter;

    public MessageHandler(Clock clock,
        OrbitClientConfig config,
        ConnectionHandler connectionHandler,
        InvocationSystem invocationSystem,
        ILoggerFactory loggerFactory)
    {
        _awaitingResponse = new ConcurrentDictionary<long, ResponseEntry>();
        _awaitingSubject = new ConcurrentDictionary<long, Completion>();
        _messageCounter = new AtomicReference<long>(0);
        _clock = clock;
        _config = config;
        _connectionHandler = connectionHandler;
        _invocationSystem = invocationSystem;
        _logger = loggerFactory.CreateLogger<MessageHandler>();
    }

    private long MessageTimeoutMs => (long)_config.MessageTimeout.TotalMilliseconds;

    public async Task OnMessage(Message message)
    {
        switch (message.Content)
        {
            //s->c  
            case MessageContent.Error:
            case MessageContent.InvocationResponse:
            case MessageContent.InvocationResponseError:
            {
                var messageId = message.MessageId.Value;
                var completion = GetCompletion(messageId);

                if (completion != null)
                {
                    switch (message.Content)
                    {
                        case MessageContent.Error:
                            completion.SetException(new RemoteException("Exceptional response received: " +
                                                                        ((MessageContent.Error)message.Content)
                                                                        .Description));
                            break;

                        case MessageContent.InvocationResponseError:
                            _invocationSystem.OnInvocationPlatformErrorResponse(
                                (MessageContent.InvocationResponseError)message.Content, completion);
                            break;
                        case MessageContent.InvocationResponse:
                            _logger.LogDebug($"InvocationResponse {message.Destination} -> {message.MessageId} ");
                            _invocationSystem.OnInvocationResponse((MessageContent.InvocationResponse)message.Content,
                                completion);
                            break;
                    }
                }
            }


                break;
            case MessageContent.InvocationRequest content:
//c->s
                _logger.LogDebug($"InvocationRequest {message.Destination} -> {message.MessageId} ");

                if (content.Reason == InvocationReason.Invocation)
                {
                    await _invocationSystem.OnInvocationRequest(message);
                }

                if (content.Reason == InvocationReason.Subscribe)
                {
                    await _invocationSystem.OnReactiveSubscribeRequest(message);
                }

                if (content.Reason == InvocationReason.UnSubscribe)
                {
                    await _invocationSystem.OnReactiveUnSubscribeRequest(message);
                }

                break;
        }
    }


    public void SendMessage(Message msg, Completion completion)
    {
        var messageId = msg.MessageId ?? _messageCounter.AtomicSet(c => c + 1);
        msg.MessageId = messageId;
        //todo 
        var newMsg = msg;
        var entry = new ResponseEntry
        {
            MessageId = messageId,
            Msg = newMsg,
            Completion = completion,
            TimeAdded = _clock.CurrentTime
        };

        if (newMsg.Content is MessageContent.InvocationRequest)
        {
            _awaitingResponse[messageId] = entry;
        }
        else
        {
            entry.Completion.SetResult(null);
        }

        _connectionHandler.Send(newMsg);
    }

    public void SendMessage(Message msg)
    {
        var messageId = msg.MessageId ?? _messageCounter.AtomicSet(c => c + 1);
        msg.MessageId = messageId;
        //todo 
        var newMsg = msg;

        _connectionHandler.Send(newMsg);
    }

    public void Tick()
    {
        _logger.LogWarning("Tick");
        // foreach (var entry in _awaitingResponse.Values)
        // {
        //  
        //     if (entry.TimeAdded < _clock.CurrentTime - MessageTimeoutMs)
        //     {
        //         _awaitingResponse.Remove(entry.MessageId, out _);
        //         var content =
        //             $"MessageId {entry.MessageId} Response timed out after {_clock.CurrentTime - entry.TimeAdded} ms, timeout is {MessageTimeoutMs}ms. " +
        //             entry.Msg;
        //         _logger.LogWarning(content);
        //         entry.Completion.SetException(new TimeoutException(content));
        //     }
        // }
    }


    public Completion? GetCompletion(long messageId)
    {
        if (_awaitingResponse.TryGetValue(messageId, out var c))
        {
            _awaitingResponse.Remove(messageId, out _);
            return c.Completion;
        }

        if (_awaitingSubject.TryGetValue(messageId, out var cc))
        {
            return cc;
        }

        _logger.LogWarning("Response for unknown message " + messageId + " received. Did it time out? (> " +
                           MessageTimeoutMs + "ms).");
        throw new Exception("Response for unknown message " + messageId + " received. Did it time out? (> " +
                            MessageTimeoutMs + "ms).");
    }

    private class ResponseEntry
    {
        public Completion Completion;
        public long MessageId;
        public Message Msg;
        public TimeMs TimeAdded;
    }

    public void UnSubscribe(long key)
    {
        _awaitingSubject.Remove(key, out _);
    }

    public void Subscribe(long key, Completion c)
    {
        _awaitingSubject.TryAdd(key, c);
    }
}