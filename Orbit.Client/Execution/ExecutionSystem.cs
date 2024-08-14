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
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<ExecutionSystem>();
        _executionLeases = executionLeases;
        _definitionDirectory = definitionDirectory;
        _componentContainer = componentContainer;
        _clock = clock;
        _addressableConstructor = addressableConstructor;
        _defaultDeactivator = defaultDeactivator;
        _localNode = localNode;
        _config = config;

        _deactivationTimeoutMs = (long)config.DeactivationTimeout.TotalMilliseconds;
        _defaultTtl = (long)config.AddressableTtl.TotalMilliseconds;
    }

    private ClientState ClientState => _localNode.Status.ClientState;

    public async Task HandleInvocation(AddressableInvocation invocation, Completion completion)
    {
        try
        {
            _logger.LogWarning("invocation " + invocation.Method);
            await _executionLeases.GetOrRenewLease(invocation.Reference);
            _logger.LogWarning("GetOrRenewLease invocation " + invocation.Method);

            if (!_activeAddressables.TryGetValue(invocation.Reference, out var handle))
            {
            }


            if (ClientState == ClientState.Stopping && (handle == null || !handle.Active))
            {
                completion.SetException(
                    new RerouteMessageException("Client is stopping, message should be routed to a new node."));
                return;
            }

            if (handle == null)
            {
                //todo
                _logger.LogWarning("handle == null " + invocation.Method);
                handle = Activate(invocation.Reference);
                await handle.Activate();
                _logger.LogWarning("handle2 == null " + invocation.Method);
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
            _clock.InPast(_executionLeases.GetLease(handle.Value.Reference).RenewAt.ToDateTime())).ToList();

        if (deactivate.Any() || expired.Any())
        {
            _logger.LogDebug($"Execution system tick: {expired} expired, {deactivate.Count()} deactivating.");
        }

        foreach (var handle in deactivate)
        {
            await Deactivate(handle.Value, DeactivationReason.ExternallyTriggered);
        }

        foreach (var handle in expired)
        {
            var lease = _executionLeases.GetLease(handle.Key);
            if (lease != null)
            {
                try
                {
                    _logger.LogInformation($"RenewLease  [{handle.Key}] Now [{_clock.Now()}]  ExpiresAt [{lease.ExpiresAt}]");
                    await _executionLeases.RenewLease(handle.Key);
                }
                catch (Exception t)
                {
                    _logger.LogError($"Unexpected error renewing lease {t.Message}");
                    await Deactivate(handle.Value, DeactivationReason.LeaseRenewalFailed);
                }
            }
            else
            {
                _logger.LogError("No lease found for ${handle.Value.Reference}");
                await Deactivate(handle.Value, DeactivationReason.LeaseRenewalFailed);
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

            var addressables = _activeAddressables.Values.Select(v => (IDeactivatable)v).ToList();
            await deactivatorToUse.Deactivate(addressables, deactivate);

            while (_activeAddressables.Count > 0)
            {
            }
        }
    }


    private ExecutionHandle Activate(AddressableReference reference)
    {
        try
        {
            //lock (_activeAddressables)
            {
                var implDefinition = _definitionDirectory.GetImplDefinition(reference.Type);
                var handle = GetOrCreateAddressable(reference, implDefinition);
                return handle;
            }
        }
        catch (InvalidOperationException e)
        {
            throw e;
        }
    }

    private async Task Deactivate(IDeactivatable deactivatable, DeactivationReason deactivationReason)
    {
        var deactivatableTask = deactivatable.Deactivate(deactivationReason);
        var delayTask = Task.Delay((int)_deactivationTimeoutMs);
        var resultTask = await Task.WhenAny(deactivatableTask, delayTask);
        //.WithCancellation(cancellationTokenSource.Token);

        if (resultTask == delayTask)
        {
            _logger.LogError(
                $"A timeout occurred (> {_deactivationTimeoutMs} ms) during deactivation of " +
                $"{deactivatable.Reference}. This addressable is now considered deactivated, this may cause state " +
                "corruption."
            );
        }

        await _executionLeases.AbandonLease(deactivatable.Reference);
        _activeAddressables.TryRemove(deactivatable.Reference, out _);
    }

    private ExecutionHandle GetOrCreateAddressable(AddressableReference reference,
        AddressableImplDefinition implDefinition)
    {
        var ad = _activeAddressables.GetOrAdd(reference, _ =>
        {
            var newInstance = CreateInstance(implDefinition.ImplClass);
            return CreateHandle(reference, implDefinition, newInstance);
        });
        _logger.LogDebug("" + _activeAddressables.Count);
        return ad;
    }

    private ExecutionHandle CreateHandle(AddressableReference reference, AddressableImplDefinition implDefinition,
        IAddressable instance)
    {
        return new ExecutionHandle(instance, reference, implDefinition, _componentContainer, _loggerFactory);
    }

    private IAddressable CreateInstance(AddressableClass addressableClass)
    {
        return _addressableConstructor.ConstructAddressable(addressableClass);
    }
}