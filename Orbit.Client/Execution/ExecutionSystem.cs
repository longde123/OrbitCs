using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Orbit.Client.Addressable;
using Orbit.Client.Net;
using Orbit.Shared.Addressable;
using Orbit.Shared.OrException;
using Orbit.Util.Di;
using Orbit.Util.Time;

namespace Orbit.Client.Execution;

using AddressableClass = Type;

public class ExecutionSystem
{
    private readonly ConcurrentDictionary<AddressableReference, ExecutionHandle> _activeAddressables = new();
    private readonly IAddressableConstructor _addressableConstructor;
    private readonly Clock _clock;
    private readonly ComponentContainer _componentContainer;
    private readonly OrbitClientConfig _config;
    private readonly long _deactivationTimeoutMs;
    private readonly AddressableDeactivator _defaultDeactivator;
    private readonly long _defaultTtl;
    private readonly AddressableDefinitionDirectory _definitionDirectory;
    private readonly ExecutionLeases _executionLeases;
    private readonly LocalNode _localNode;
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;

    public ExecutionSystem(ExecutionLeases executionLeases, AddressableDefinitionDirectory definitionDirectory,
        ComponentContainer componentContainer, Clock clock, IAddressableConstructor addressableConstructor,
        AddressableDeactivator defaultDeactivator, LocalNode localNode, OrbitClientConfig config,
        ILoggerFactory loggerFactory)
    {
        this._loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<ExecutionSystem>();
        this._executionLeases = executionLeases;
        this._definitionDirectory = definitionDirectory;
        this._componentContainer = componentContainer;
        this._clock = clock;
        this._addressableConstructor = addressableConstructor;
        this._defaultDeactivator = defaultDeactivator;
        this._localNode = localNode;
        this._config = config;

        _deactivationTimeoutMs = (long)config.DeactivationTimeout.TotalMilliseconds;
        _defaultTtl = (long)config.AddressableTtl.TotalMilliseconds;
    }

    private ClientState ClientState => _localNode.Status.ClientState;

    public async Task HandleInvocation(AddressableInvocation invocation, Completion completion)
    {
        try
        {
            await _executionLeases.GetOrRenewLease(invocation.Reference);
            _activeAddressables.TryGetValue(invocation.Reference, out var handle);
            if (ClientState == ClientState.Stopping && (handle == null || !handle.Active))
            {
                completion.SetException(
                    new RerouteMessageException("Client is stopping, message should be routed to a new node."));
                return;
            }

            if (handle == null)
            {
                handle = await Activate(invocation.Reference);
            }

            if (handle == null)
            {
                throw new Exception($"No active addressable found for {invocation.Reference}");
            }

            var result = await handle.Invoke(invocation);


            completion.SetResult(result);
        }
        catch (Exception t)
        {
            completion.SetException(t);
        }
    }

    public static (List<T>, List<T>) Partition<T>(IEnumerable<T> source, Func<T, bool> predicate)
    {
        var first = new List<T>();
        var second = new List<T>();
        foreach (var element in source)
        {
            if (predicate(element))
            {
                first.Add(element);
            }
            else
            {
                second.Add(element);
            }
        }

        return (first, second);
    }

    public async Task Tick()
    {
        _logger.LogWarning("Tick");
        var (deactivate, active) = Partition(_activeAddressables,
            handle =>
            {
                return handle.Value.DeactivateNextTick ||
                       _clock.CurrentTime - handle.Value.LastActivity > _defaultTtl ||
                       _executionLeases.GetLease(handle.Value.Reference) == null;
            });

        var expired = active.Where(handle =>
            _clock.InPast(_executionLeases.GetLease(handle.Value.Reference).RenewAt.ToDateTime()));

        if (deactivate.Any() || expired.Any())
        {
            _logger.LogDebug($"Execution system tick: {expired} expired, {deactivate.Count()} deactivating.");
        }

        foreach (var handle in _activeAddressables)
        {
            if (handle.Value.DeactivateNextTick)
            {
                Deactivate(handle.Value, DeactivationReason.ExternallyTriggered);
                continue;
            }

            if (_clock.CurrentTime - handle.Value.LastActivity > _defaultTtl)
            {
                Deactivate(handle.Value, DeactivationReason.TtlExpired);
                continue;
            }

            var lease = _executionLeases.GetLease(handle.Key);
            if (lease != null)
            {
                if (_clock.InPast(lease.RenewAt.ToDateTime()))
                {
                    try
                    {
                        await _executionLeases.RenewLease(handle.Key);
                    }
                    catch (Exception t)
                    {
                        _logger.LogError($"Unexpected error renewing lease {t}");
                        Deactivate(handle.Value, DeactivationReason.LeaseRenewalFailed);
                    }
                }
            }
            else
            {
                _logger.LogError("No lease found for ${handle.Value.Reference}");
                Deactivate(handle.Value, DeactivationReason.LeaseRenewalFailed);
            }
        }

        if (deactivate.Any() || expired.Any())
        {
            // logger.debug { "Execution system tick: end" }
        }
    }

    public async Task Stop(AddressableDeactivator? deactivator = null)
    {
        if (_activeAddressables.Count > 0)
        {
            _logger.LogInformation($"Draining {_activeAddressables.Count()} addressables");

            var deactivatorToUse = deactivator ?? _defaultDeactivator;
            Deactivator deactivate = async a => { await Deactivate(a, DeactivationReason.NodeShuttingDown); };
            await deactivatorToUse.Deactivate(_activeAddressables.Values.Select(v => (IDeactivatable)v).ToList(),
                deactivate);

            while (_activeAddressables.Count > 0)
            {
            }
        }
    }

    private async Task<ExecutionHandle> Activate(AddressableReference reference)
    {
        try
        {
            var implDefinition = _definitionDirectory.GetImplDefinition(reference.Type);
            var handle = GetOrCreateAddressable(reference, implDefinition);
            await handle.Activate();
            return handle;
        }
        catch (InvalidOperationException e)
        {
            throw e;
        }
    }

    private async Task Deactivate(IDeactivatable deactivatable, DeactivationReason deactivationReason)
    {
        try
        {
            await Task.Delay((int)_deactivationTimeoutMs);
            await deactivatable.Deactivate(deactivationReason);
        }
        catch (TimeoutException e)
        {
            _logger.LogError(e.ToString());
        }

        await _executionLeases.AbandonLease(deactivatable.Reference);
        _activeAddressables.TryRemove(deactivatable.Reference, out _);
    }

    private ExecutionHandle GetOrCreateAddressable(AddressableReference reference,
        AddressableImplDefinition implDefinition)
    {
        return _activeAddressables.GetOrAdd(reference, _ =>
        {
            var newInstance = CreateInstance(implDefinition.ImplClass);
            return CreateHandle(reference, implDefinition, newInstance);
        });
    }

    private ExecutionHandle CreateHandle(AddressableReference reference, AddressableImplDefinition implDefinition,
        Addressable.IAddressable instance)
    {
        return new ExecutionHandle(instance, reference, implDefinition, _componentContainer, _loggerFactory);
    }

    private Addressable.IAddressable CreateInstance(AddressableClass addressableClass)
    {
        return _addressableConstructor.ConstructAddressable(addressableClass);
    }
}