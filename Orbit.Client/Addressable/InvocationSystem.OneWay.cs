namespace Orbit.Client.Addressable;

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Execution;
using Net;
using Reactive;
using Util;
using Orbit.Shared.Addressable;
using Orbit.Shared.Net;
using Shared.OrException;
using Orbit.Util.Concurrent;
using Orbit.Util.Di;
using AddressableType = String;
using AddressableInvocationArgument = Tuple<object, Type>;
using AddressableInvocationArguments = List<Tuple<object, Type>>;

public partial class InvocationSystem
{
    private readonly ConcurrentDictionary<long, object> _awaitingSubscription;

    public void SendUnSubscribe(AddressableInvocation invocation, Completion completion)
    {
        var returnType = invocation.Args.First().Item1;
        var unSubscribe = _awaitingSubscribe.First(a => a.Value == returnType);

        if (unSubscribe.Value != null)
        {
            invocation.Args[0] = new AddressableInvocationArgument(new AbstractSubject()
            {
                MessageId = unSubscribe.Key
            }, invocation.Args.First().Item2);
            SendInvocation(invocation, completion);
            _messageHandler.Value.UnSubscribe(unSubscribe.Key);
            _awaitingSubscribe.Remove(unSubscribe.Key, out _);
        }
    }

    public void SendSubscribe(AddressableInvocation invocation, Completion completion)
    {
        if (_localNode.Status.ClientState != ClientState.Connected)
        {
            throw new InvalidOperationException("The Orbit client is not connected");
        }

        var returnType = invocation.Args.First().Item2;
        var observer = (IAsyncObserver)invocation.Args.First().Item1;

        var c = new Completion();
        c.Task.CastTo(returnType.GenericTypeArguments[0], observer);
        _messageHandler.Value.MessageCounter.AtomicSet(c => c + 1);
        var abstractSubject = new AbstractSubject()
        {
            MessageId = _messageHandler.Value.MessageCounter.Get()
        };
        invocation.Args[0] = new AddressableInvocationArgument(abstractSubject, invocation.Args.First().Item2);
        var arguments = _serializer.Serialize(invocation.Args);
        var msg = new Message
        {
            Content = new MessageContent.InvocationRequest(invocation.Reference, invocation.Method, arguments,
                invocation.Reason)
        };
        _messageHandler.Value.SendMessage(msg, completion);
        _awaitingSubscribe.TryAdd(abstractSubject.MessageId, observer);
        _messageHandler.Value.Subscribe(abstractSubject.MessageId, c);
    }


    public async Task OnReactiveSubscribeRequest(Message msg)
    {
        var content = msg.Content as MessageContent.InvocationRequest;

        if (!_awaitingSubscription.TryGetValue(msg.MessageId.Value, out var actSubscribe))
        {
            var arguments = _serializer.Deserialize<AddressableInvocationArguments>(content.Arguments);
            var abstractSubject = (AbstractSubject)arguments[0].Item1;
            var act = new AsyncSubscribe<object>((result) =>
            {
                var resultSerialize = _serializer.Serialize(result);
                var response = new MessageContent.InvocationResponse(resultSerialize);
                _messageHandler.Value.SendMessage(new Message
                {
                    MessageId = abstractSubject.MessageId,
                    Target = new MessageTarget.Unicast(msg.Source),
                    Content = response
                });
            }, (Exception t) =>
            {
                var resultSerialize = _serializer.Serialize(t);
                var response = new MessageContent.InvocationResponseError()
                {
                    Platform = resultSerialize,
                    Description = ""
                };
                _messageHandler.Value.SendMessage(new Message
                {
                    MessageId = abstractSubject.MessageId,
                    Target = new MessageTarget.Unicast(msg.Source),
                    Content = response
                });
            });

            actSubscribe = act.CastTo(arguments[0].Item2.GenericTypeArguments[0]);

            _awaitingSubscription.TryAdd(abstractSubject.MessageId, actSubscribe);

            arguments[0] = new Tuple<object, Type>(actSubscribe, arguments[0].Item2);
            var invocation = new AddressableInvocation
            {
                Reference = content.Destination,
                Method = content.Method,
                Args = arguments
            };

            //Args  T -> Repose
            //IAsyncObserver<T> subject     Message->T 
            // T -> Repose  msg 
            var completion = new Completion();
            await _executionSystem.HandleInvocation(invocation, completion);
            var result = await completion.Task;
            MessageContent response;
            if (null == result)
            {
                response = new MessageContent.InvocationResponse("null");
            }
            else
            {
                var resultSerialize = _serializer.Serialize(result);
                response = new MessageContent.InvocationResponse(resultSerialize);
            }

            _messageHandler.Value.SendMessage(new Message
            {
                MessageId = msg.MessageId,
                Target = new MessageTarget.Unicast(msg.Source),
                Content = response
            });
        }
        else
        {
            //todo
        }
    }

    public async Task OnReactiveUnSubscribeRequest(Message msg)
    {
        var content = msg.Content as MessageContent.InvocationRequest;
        var arguments = _serializer.Deserialize<AddressableInvocationArguments>(content.Arguments);

        var abstractSubject = (AbstractSubject)arguments[0].Item1;
        if (_awaitingSubscription.TryRemove(abstractSubject.MessageId, out var actSubscribe))
        {
            arguments[0] = new Tuple<object, Type>(actSubscribe, arguments[0].Item2);
            var invocation = new AddressableInvocation
            {
                Reference = content.Destination,
                Method = content.Method,
                Args = arguments
            };
            var completion = new Completion();
            await _executionSystem.HandleInvocation(invocation, completion);
            var result = await completion.Task;
            MessageContent response;
            if (null == result)
            {
                response = new MessageContent.InvocationResponse("null");
            }
            else
            {
                var resultSerialize = _serializer.Serialize(result);
                response = new MessageContent.InvocationResponse(resultSerialize);
            }

            _messageHandler.Value.SendMessage(new Message
            {
                MessageId = msg.MessageId,
                Target = new MessageTarget.Unicast(msg.Source),
                Content = response
            });
        }
        else
        {
            //todo
        }
    }
}