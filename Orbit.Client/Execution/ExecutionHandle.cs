using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Orbit.Client.Addressable;
using Orbit.Client.Net;
using Orbit.Client.Util;
using Orbit.Shared.Addressable;
using Orbit.Shared.Net;
using Orbit.Shared.OrException;
using Orbit.Util.Concurrent;
using Orbit.Util.Di;
using Orbit.Util.Time;

namespace Orbit.Client.Execution;

public class ExecutionHandle : IDeactivatable
{
    private readonly int _addressableBufferCount = 128;
    private readonly Channel<EventType> _channel;
    private readonly Lazy<Clock> _clock;
    private readonly ComponentContainer _componentContainer;

    private readonly long _createdTime;
    private readonly AddressableImplDefinition _implDefinition;
    private readonly Addressable.IAddressable _instance;
    private readonly Lazy<InvocationSystem> _invocationSystem;
    private readonly AtomicReference<long> _lastActivityAtomic;

    private readonly ILogger _logger;

    private readonly Lazy<OrbitClient> _orbitClient;
    private readonly Lazy<SupervisorScope> _supervisorScope;
    private readonly CancellationTokenSource _tokenSource;
    private volatile bool _deactivateNextTick = false;

    public ExecutionHandle(Addressable.IAddressable instance, AddressableReference reference,
        AddressableImplDefinition implDefinition, ComponentContainer componentContainer, ILoggerFactory loggerFactory
    )
    {
        _logger = loggerFactory.CreateLogger<ExecutionHandle>();
        this._instance = instance;
        Reference = reference;
        this._implDefinition = implDefinition;
        this._componentContainer = componentContainer;

        _orbitClient = componentContainer.Inject<OrbitClient>();
        _clock = componentContainer.Inject<Clock>();
        _supervisorScope = componentContainer.Inject<SupervisorScope>();
        _invocationSystem = componentContainer.Inject<InvocationSystem>();


        _createdTime = _clock.Value.CurrentTime;

        _lastActivityAtomic = new AtomicReference<long>(_createdTime);
        _channel = Channel.CreateUnbounded<EventType>(); //addressableBufferCount;


        if (instance is AbstractAddressable inst)
        {
            inst.Context = new AddressableContext(reference, _orbitClient.Value);
        }

        _tokenSource = new CancellationTokenSource();
        Task.Run(async () => await Worker(), _tokenSource.Token);
    }

    public bool DeactivateNextTick => _deactivateNextTick;
    public long LastActivity => _lastActivityAtomic.Get();
    public bool Active { get; private set; }

    public AddressableReference Reference { get; }

    public async Task Deactivate(DeactivationReason deactivationReason)
    {
        var completion = new Completion();
        SendEvent(new DeactivateEvent(deactivationReason, completion));
        await completion.Task;
    }

    public async Task Activate()
    {
        var completion = new Completion();
        SendEvent(new ActivateEvent(completion));
        await completion.Task;
    }

    public async Task<object?> Invoke(AddressableInvocation invocation)
    {
        var completion = new Completion();
        SendEvent(new InvokeEvent(invocation, completion));
        return await completion.Task;
    }

    private async void SendEvent(EventType eventType)
    {
        var writer = _channel.Writer;
        try
        {
            await writer.WriteAsync(eventType);
        }
        catch (Exception e)
        {
            var errMsg = $"Buffer capacity exceeded (>{_addressableBufferCount}) for {Reference}";
            _logger.LogError(errMsg);
            throw new CapacityExceededException(errMsg);
        }
    }

    private async Task OnActivate()
    {
        _logger.LogDebug($"Activating {Reference}...");
        var stopwatch = Stopwatch.Start(_clock.Value);
        {
            if (_implDefinition.OnActivateMethod != null)
            {
                var method = _implDefinition.OnActivateMethod;
                if (DeferredWrappers.IsAsync(method.Method))
                {
                    await DeferredWrappers.WrapSuspend(method.Method, _instance);
                }
                else
                {
                    method.Method.Invoke(_instance, null);
                }
            }
        }
        var elapsed = stopwatch.Elapsed;
        _logger.LogDebug($"Activated {Reference} in {elapsed}ms.");
        Active = true;
    }

    private async Task<object?> OnInvoke(AddressableInvocation invocation)
    {
        _lastActivityAtomic.AtomicSet(a => _clock.Value.CurrentTime);

        try
        {
            var result = await MethodInvoker.Invoke(_instance, invocation.Method, invocation.Args);
            return result;
        }
        catch (Exception ite)
        {
            //todo
            throw ite;
        }
    }

    private async Task OnDeactivate(DeactivationReason deactivationReason)
    {
        _logger.LogDebug($"Deactivating {Reference}...");
        var stopwatch = Stopwatch.Start(_clock.Value);
        {
            if (_implDefinition.OnDeactivateMethod != null)
            {
                var method = _implDefinition.OnDeactivateMethod;
                var isSuspended = DeferredWrappers.IsAsync(method.Method);


                var hasReason = method.Method.GetParameters().FirstOrDefault().ParameterType ==
                                typeof(DeactivationReason);

                object[] reasonArgs = null;

                if (hasReason)
                {
                    reasonArgs = new object[] { deactivationReason };
                }


                try
                {
                    if (isSuspended)
                    {
                        await DeferredWrappers.WrapSuspend(method.Method, _instance, reasonArgs);
                    }
                    else
                    {
                        method.Method.Invoke(_instance, reasonArgs);
                    }
                }
                catch (InvocationTargetException ite)
                {
                    _logger.LogWarning($"Exception caught on actor deactivation {ite}");
                }
            }

            _tokenSource.Cancel();
            DrainChannel();
        }
        var elapsed = stopwatch.Elapsed;
        _logger.LogDebug($"Deactivated {Reference} in {elapsed}ms.");
        Active = false;
    }

    private async Task DrainChannel()
    {
        await foreach (var ent in _channel.Reader.ReadAllAsync())
        {
            if (ent is InvokeEvent e)
            {
                _logger.LogWarning(
                    $"Received invocation which can no longer be handled locally. Rerouting... {e.Invocation}");
                e.Invocation.Reason = InvocationReason.Rerouted;
                _invocationSystem.Value.SendInvocation(e.Invocation, new Completion());
            }
        }
    }

    //todo 
    private async Task Worker()
    {
        await foreach (var ent in _channel.Reader.ReadAllAsync())
        {
            try
            {
                object result = null;
                switch (ent)
                {
                    case ActivateEvent activateEvent:
                        _logger.LogDebug($"ActivateEvent {activateEvent}");
                        await OnActivate();
                        break;
                    case InvokeEvent invokeEvent:
                        _logger.LogDebug($"InvokeEvent {invokeEvent.Invocation}");
                        result = await OnInvoke(invokeEvent.Invocation);

                        break;
                    case DeactivateEvent deactivateEvent:
                        _logger.LogDebug($"DeactivateEvent {deactivateEvent}");
                        await OnDeactivate(deactivateEvent.DeactivationReason);
                        break;
                }

                ent.Completion.SetResult(result);
            }
            catch (Exception ex)
            {
                ent.Completion.SetException(ex);
            }
        }
    }

    public abstract class EventType
    {
        public EventType(Completion completion)
        {
            Completion = completion;
        }

        public Completion Completion { get; }
    }

    public class ActivateEvent : EventType
    {
        public ActivateEvent(Completion completion) : base(completion)
        {
        }
    }

    public class InvokeEvent : EventType
    {
        public InvokeEvent(AddressableInvocation invocation, Completion completion) : base(completion)
        {
            Invocation = invocation;
        }

        public AddressableInvocation Invocation { get; }
    }

    public class DeactivateEvent : EventType
    {
        public DeactivateEvent(DeactivationReason deactivationReason, Completion completion) : base(completion)
        {
            DeactivationReason = deactivationReason;
        }

        public DeactivationReason DeactivationReason { get; }
    }
}