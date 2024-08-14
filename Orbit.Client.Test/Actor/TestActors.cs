using Orbit.Client.Addressable;
using Orbit.Client.Net;
using Orbit.Client.Reactive;
using Orbit.Shared.Addressable;
using Orbit.Shared.Mesh;

namespace Orbit.Client.Actor;

public interface ITestActorSubject : IActorSubject<string>
{
}

public class TestActorSubject : AbstractActorSubject<string>, ITestActorSubject
{
}

public static class TrackingGlobals
{
    private static readonly ReaderWriterLockSlim Rwlock = new();

    public static int DeactivateTestCounts;
    public static int ConcurrentDeactivations { get; private set; }
    public static int MaxConcurrentDeactivations { get; private set; }
    public static List<Key> DeactivatedActors { get; } = new();

    public static void Reset()
    {
        DeactivateTestCounts = 0;
        ConcurrentDeactivations = 0;
        MaxConcurrentDeactivations = 0;
        DeactivatedActors.Clear();
    }

    public static void Deactivate()
    {
        StartDeactivate();
        EndDeactivate();
    }

    public static void StartDeactivate()
    {
        Rwlock.EnterWriteLock();
        try
        {
            ConcurrentDeactivations++;
        }
        finally
        {
            Rwlock.ExitWriteLock();
        }
    }

    public static void EndDeactivate()
    {
        Rwlock.EnterWriteLock();
        try
        {
            DeactivateTestCounts++;
            MaxConcurrentDeactivations = Math.Max(MaxConcurrentDeactivations, ConcurrentDeactivations);
            ConcurrentDeactivations--;
        }
        finally
        {
            Rwlock.ExitWriteLock();
        }
    }
}

public interface IGreeterActor : IActorWithNoKey
{
    Task<string> GreetAsync(string name);
}

public class GreeterActorImpl : IGreeterActor
{
    public async Task<string> GreetAsync(string name)
    {
        return await Task.FromResult($"Hello {name}");
    }
}

public interface ITimeoutActor : IActorWithNoKey
{
    Task Timeout();
}

public class TimeoutActorImpl : ITimeoutActor
{
    public async Task Timeout()
    {
        await Task.CompletedTask;
        await new Completion().Task;
    }
}

public interface IActorWithNoImpl : IActorWithNoKey
{
    Task<string> GreetAsync(string name);
}

public interface IComplexDtoActor : IActorWithNoKey
{
    Task ComplexCall(ComplexDto dto);

    public class ComplexDto
    {
        public ComplexDto(string blah)
        {
            Blah = blah;
        }

        public string Blah { get; set; }
    }
}

public class ComplexDtoActorImpl : IComplexDtoActor
{
    public async Task ComplexCall(IComplexDtoActor.ComplexDto dto)
    {
        await Task.CompletedTask;
    }
}

public interface INcrementActor : IActorWithNoKey
{
    Task<long> Increment();
}

public class IncrementActorImpl : INcrementActor
{
    private long _counter = 1L;

    public async Task<long> Increment()
    {
        return await Task.FromResult(++_counter);
    }
}

public class TestException : Exception
{
    public TestException(string msg) : base(msg)
    {
    }
}

public interface IThrowingActor : IActorWithNoKey
{
    Task<long> DoThrow();
}

public class ThrowingActorImpl : IThrowingActor
{
    public async Task<long> DoThrow()
    {
        throw new TestException("Threw");
    }
}

public interface IDActor : IActorWithStringKey
{
    Task<string> GetId();
}

public class IdActorImpl : AbstractActor, IDActor
{
    public async Task<string> GetId()
    {
        var stringKey = Context.Reference.Key as Key.StringKey;
        return await Task.FromResult(stringKey.Key);
    }
}

public class ComplexNull
{
    public string Greeting { get; set; }
}

public interface INullActor : IActorWithNoKey
{
    Task<string> SimpleNull(string arg1, string arg2);
    Task<string> ComplexNull(string arg1, ComplexNull arg2);
}

public class NullActorImpl : INullActor
{
    public async Task<string> SimpleNull(string arg1, string arg2)
    {
        return await Task.FromResult(arg1 + arg2);
    }

    public async Task<string> ComplexNull(string arg1, ComplexNull? arg2)
    {
        var result = arg1 + (arg2 != null ? arg2.Greeting : "null");
        return await Task.FromResult(result);
    }
}

public interface IClientAwareActor : IActorWithStringKey
{
    Task<NodeId> GetClient();
}

public class ClientAwareActorImpl : AbstractActor, IClientAwareActor
{
    public async Task<NodeId> GetClient()
    {
        return await Task.FromResult(Context.Client.NodeId);
    }

    [OnDeactivate]
    public async Task OnDeactivate()
    {
        Console.WriteLine($"Deactivating actor {Context.Reference.Key}");
        TrackingGlobals.DeactivatedActors.Add(Context.Reference.Key);
        await Task.CompletedTask;
    }
}

public interface IBasicOnDeactivate : IActorWithNoKey
{
    Task<string> GreetAsync(string name);
}

public class BasicOnDeactivateImpl : IBasicOnDeactivate
{
    public async Task<string> GreetAsync(string name)
    {
        return await Task.FromResult($"Hello {name}");
    }

    [OnDeactivate]
    public async Task OnDeactivate()
    {
        Interlocked.Increment(ref TrackingGlobals.DeactivateTestCounts);
        await Task.CompletedTask;
    }
}

public interface IArgumentOnDeactivate : IActorWithNoKey
{
    Task<string> GreetAsync(string name);
}

public class ArgumentOnDeactivateImpl : IArgumentOnDeactivate
{
    public async Task<string> GreetAsync(string name)
    {
        return await Task.FromResult($"Hello {name}");
    }

    [OnDeactivate]
    public async Task OnDeactivate(DeactivationReason deactivationReason)
    {
        Interlocked.Increment(ref TrackingGlobals.DeactivateTestCounts);
        await Task.CompletedTask;
    }
}

public interface IKeyedDeactivatingActor : IActorWithInt32Key
{
    Task Ping();
}

public class KeyedDeactivatingActorImpl : IKeyedDeactivatingActor
{
    public async Task Ping()
    {
        await Task.CompletedTask;
    }

    [OnDeactivate]
    public async Task OnDeactivate(DeactivationReason deactivationReason)
    {
        Interlocked.Increment(ref TrackingGlobals.DeactivateTestCounts);
        await Task.CompletedTask;
    }
}

public interface ISlowDeactivateActor : IActorWithInt32Key
{
    Task<string> Ping(string msg = "");
}

public class SlowDeactivateActorImpl : ISlowDeactivateActor
{
    public async Task<string> Ping(string msg)
    {
        return await Task.FromResult(msg);
    }

    [OnDeactivate]
    public async Task OnDeactivate(DeactivationReason deactivationReason)
    {
        TrackingGlobals.StartDeactivate();
        await Task.Delay(new Random().Next(50) + 50);
        TrackingGlobals.EndDeactivate();
    }
}

public interface IThrowsOnDeactivateActor : IActorWithNoKey
{
    Task Ping();
}

public class ThrowsOnDeactivateActorImpl : IThrowsOnDeactivateActor
{
    public async Task Ping()
    {
        throw new TestException("Throwing on Deactivation");
    }

    [OnDeactivate]
    public async Task OnDeactivate(DeactivationReason deactivationReason)
    {
        Console.WriteLine("Throwing on deactivation");
        TrackingGlobals.Deactivate();
        throw new TestException("Throwing on Deactivation");
    }
}

public interface ISuspendingMethodActor : IActorWithStringKey
{
    Task<string> Ping(string msg = "");
    Task Fail();
}

public class SuspendingMethodActorImpl : ISuspendingMethodActor
{
    public async Task<string> Ping(string msg)
    {
        return await Task.FromResult(msg);
    }

    public async Task Fail()
    {
        throw new InvalidOperationException("Intentionally thrown test exception");
    }

    [OnActivate]
    public async Task OnActivate()
    {
        await Task.Delay(1);
        Console.WriteLine("Activated");
    }

    [OnDeactivate]
    public async Task OnDeactivate(DeactivationReason deactivationReason)
    {
        await Task.Delay(1);
        Console.WriteLine($"Deactivated: {deactivationReason}");
        TrackingGlobals.EndDeactivate();
    }
}