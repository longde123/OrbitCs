using NUnit.Framework;
using Orbit.Client.Actor;
using Orbit.Util.Misc;

namespace Orbit.Client.Test;

public class BasicActorTests : BaseIntegrationTest
{
    [Test]
    public async Task TestBasicActorRequestResponse()
    {
        var actor = Client.ActorFactory.CreateProxy<IGreeterActor>();
        var result = await actor.GreetAsync("Joe");
        Assert.AreEqual("Hello Joe", result);
    }

    [Test]
    public async Task TestBasicActorRequestResponseConcurrent()
    {
        var tasks = new List<Task<string>>();

        for (var i = 0; i < 100; i++)
            //for (var i = 0; i < 100; i++)
        {
            var actor = Client.ActorFactory.CreateProxy<IGreeterActor>();
            tasks.Add(actor.GreetAsync("Joe"));
        }

        var results = await Task.WhenAll(tasks);
        foreach (var result in results)
        {
            Assert.AreEqual("Hello Joe", result);
        }
    }

    [Test]
    public async Task TestComplexDtoRequestResponse()
    {
        var actor = Client.ActorFactory.CreateProxy<IComplexDtoActor>();
        await actor.ComplexCall(new IComplexDtoActor.ComplexDto("Hello"));
    }

    [Test]
    public async Task EnsureThrowFails()
    {
        var actor = Client.ActorFactory.CreateProxy<IThrowingActor>();
        Assert.ThrowsAsync<TestException>(async () => await actor.DoThrow());
    }

    [Test]
    public async Task EnsureInvalidActorTypeThrows()
    {
        var actor = Client.ActorFactory.CreateProxy<IActorWithNoImpl>();
        try
        {
            await actor.GreetAsync("Daniel Jackson");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        Assert.ThrowsAsync<InvalidOperationException>(async () => await actor.GreetAsync("Daniel Jackson"));
    }

    [Test]
    public async Task EnsureMessageTimeoutThrows()
    {
        var actor = Client.ActorFactory.CreateProxy<ITimeoutActor>();
        Assert.ThrowsAsync<TimeoutException>(async () => await actor.Timeout());
    }

    [Test]
    public async Task EnsureActorReused()
    {
        var actor = Client.ActorFactory.CreateProxy<INcrementActor>();
        var result1 = await actor.Increment();
        var result2 = await actor.Increment();
        Assert.True(result2 > result1);
    }

    [Test]
    public async Task EnsureActorDeactivates()
    {
        var actor = Client.ActorFactory.CreateProxy<INcrementActor>();
        var call1 = await actor.Increment();
        await Task.Delay(TimeSpan.FromSeconds(Client.Config.AddressableTtl.TotalSeconds * 2));
        await Task.Delay(TimeSpan.FromSeconds(Client.Config.TickRate.TotalSeconds *
                                              2)); // Wait twice the tick so the deactivation should have happened
        var call2 = await actor.Increment();
        Assert.True(call2 <= call1);
    }

    [Test]
    public async Task EnsureMethodOnDeactivate()
    {
        var before = TrackingGlobals.DeactivateTestCounts;

        var argActor = Client.ActorFactory.CreateProxy<IBasicOnDeactivate>();
        await argActor.GreetAsync("Test");

        var noArgActor = Client.ActorFactory.CreateProxy<IArgumentOnDeactivate>();
        await noArgActor.GreetAsync("Test");

        await Task.Delay(TimeSpan.FromMilliseconds(Client.Config.AddressableTtl.TotalMilliseconds * 2));
        await Task.Delay(
            TimeSpan.FromMilliseconds(Client.Config.TickRate.TotalMilliseconds *
                                      2)); // Wait twice the tick so the deactivation should have happened

        var after = TrackingGlobals.DeactivateTestCounts;
        Assert.True(before < after);
    }

    [Test]
    public async Task TestActorWithIdAndContext()
    {
        var actorKey = RngUtils.RandomString(128);
        var actor = Client.ActorFactory.CreateProxy<IDActor>(actorKey);
        var result = await actor.GetId();
        Assert.AreEqual(actorKey, result);
    }

    [Test]
    public async Task TestActorWithSimpleNullArgument()
    {
        var actor = Client.ActorFactory.CreateProxy<INullActor>();
        var result = await actor.SimpleNull("Hi ", "null");
        Assert.AreEqual("Hi null", result);
    }

    [Test]
    public async Task TestActorWithComplexNullArgument()
    {
        var actor = Client.ActorFactory.CreateProxy<INullActor>();
        var result = await actor.ComplexNull("Bob ", null);
        Assert.AreEqual("Bob null", result);
    }

    [Test]
    public async Task TestPlatformException()
    {
        var customClient = await StartClient(
            nameSpace: "platformExceptionTest",
            platformExceptions: true
        );

        var actor = customClient.ActorFactory.CreateProxy<IThrowingActor>();
        Assert.ThrowsAsync<TestException>(async () => await actor.DoThrow());
    }

    [Test]
    public async Task CallingAnActorWithASuspendedMethodReturnsCorrectResult()
    {
        var actor = Client.ActorFactory.CreateProxy<ISuspendingMethodActor>("test");


        Assert.AreEqual("test message", await actor.Ping("test message"));
    }

    [Test]
    public async Task DeactivatingActorWithSuspendOnDeactivateCallsDeactivate()
    {
        var actor = Client.ActorFactory.CreateProxy<ISuspendingMethodActor>("test");
        Assert.AreEqual("test message", await actor.Ping("test message"));
        await DisconnectClient(Client);
        Assert.AreEqual(1, TrackingGlobals.DeactivateTestCounts);
    }

    [Test]
    public async Task ActorWithSuspendingMethodWithErrorThrowsException()
    {
        var actor = Client.ActorFactory.CreateProxy<ISuspendingMethodActor>("test");
        try
        {
            await actor.Fail();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        Assert.ThrowsAsync<InvalidOperationException>(async () => await actor.Fail());
        Console.WriteLine("ActorWithSuspendingMethodWithErrorThrowsException");
    }
}