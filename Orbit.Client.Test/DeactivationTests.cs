using System.Diagnostics;
using NUnit.Framework;
using Orbit.Client.Actor;
using Orbit.Client.Execution;
using Orbit.Client.Net;

namespace Orbit.Client.Test;

public class DeactivationTests : BaseIntegrationTest
{
    [Test]
    public async Task AllActorsGetDeactivatedOnShutdown()
    {
        var actorCount = 500;
        for (var key = 0; key < actorCount; key++)
        {
            var keyedDeactivatingActor = Client.ActorFactory.CreateProxy<IKeyedDeactivatingActor>(key);
            await keyedDeactivatingActor.Ping();
        }

        await DisconnectClient(Client);

        await Task.Delay(TimeSpan.FromSeconds(5)); // simulate eventually logic with delay
        Assert.AreEqual(TrackingGlobals.DeactivateTestCounts, actorCount);
    }

    [Test]
    public async Task InstantDeactivationDeactivatesAllAddressablesSimultaneously()
    {
        var client = await StartClient(
            addressableDeactivation: new AddressableDeactivator.Instant.Config()
        );

        for (var key = 0; key < 100; key++)
        {
            await client.ActorFactory.CreateProxy<ISlowDeactivateActor>(key).Ping("message");
        }

        await Task.Delay(10);
        await DisconnectClient(client);

        Assert.That(TrackingGlobals.MaxConcurrentDeactivations, Is.GreaterThan(20));
        Assert.That(TrackingGlobals.MaxConcurrentDeactivations, Is.LessThan(100));
    }

    [Test]
    public async Task ConcurrentDeactivationDoesNotExceedMaximumConcurrentSetting()
    {
        var client2 = await StartClient(
            addressableDeactivation: new AddressableDeactivator.Concurrent.Config(10)
        );

        for (var key = 0; key < 100; key++)
        {
            await client2.ActorFactory.CreateProxy<ISlowDeactivateActor>(key).Ping("message");
        }

        await Task.Delay(10);
        await DisconnectClient(client2);

        Assert.That(TrackingGlobals.MaxConcurrentDeactivations, Is.LessThanOrEqualTo(10));
    }

    [Test]
    public async Task ActorsAddedDuringDeactivationAreRejected()
    {
        var count = 100;
        for (var key = 0; key < count; key++)
        {
            await Client.ActorFactory.CreateProxy<ISlowDeactivateActor>(key).Ping("message");
        }

        await StartServer(50057, tickRate: TimeSpan.FromSeconds(5));
        var client2 = await StartClient(50057, packages: new List<string> { "orbit.client.alternativeActor" });

        for (var k = 0; k < 100; k++)
        {
            var key = k + 100;
            if (Client.Status != ClientState.Idle)
            {
                await Task.Delay(5);
                await client2.ActorFactory.CreateProxy<ISlowDeactivateActor>(key).Ping("message");
            }
        }

        await DisconnectClient(Client,
            new AddressableDeactivator.TimeSpan(new AddressableDeactivator.TimeSpan.Config(500)));

        Assert.That(TrackingGlobals.DeactivateTestCounts, Is.EqualTo(count));
    }

    [Test]
    public async Task DeactivatingByRateLimitTakesMinimumTimeWithAddressableCount()
    {
        // Only use custom client
        await DisconnectClient(this.Client);
        var count = 500;
        var client = await StartClient(
            addressableDeactivation: new AddressableDeactivator.RateLimited.Config(1000)
        );

        for (var key = 0; key < count; key++)
        {
            await client.ActorFactory.CreateProxy<IKeyedDeactivatingActor>(key).Ping();
        }

        var stopwatch = Stopwatch.StartNew();
        await DisconnectClient(client);
        stopwatch.Stop();

        Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(500));
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(700));
    }

    [Test]
    public async Task DeactivatingByTimespanTakesMinimumTimespanRegardlessOfAddressableCount()
    {
        // Only use custom client
        await DisconnectClient(this.Client);

        var key = 0;

        async Task TestAsync(int count, long deactivationTime)
        {
            Console.WriteLine($"Testing {count} deactivations in {deactivationTime}ms");
            var client =
                await StartClient(
                    addressableDeactivation: new AddressableDeactivator.TimeSpan.Config(deactivationTime));

            for (var i = 0; i < count; i++)
            {
                await client.ActorFactory.CreateProxy<IKeyedDeactivatingActor>(key++).Ping();
            }

            var stopwatch = Orbit.Util.Time.Stopwatch.Start(Clock);

            await DisconnectClient(client);

            Console.WriteLine($"Deactivated in {stopwatch.Elapsed}ms");
            Assert.GreaterOrEqual(stopwatch.Elapsed, deactivationTime * 0.95);
            Assert.Less(stopwatch.Elapsed, deactivationTime * 4);
        }

        await TestAsync(100, 500);
        await TestAsync(500, 500);
        await TestAsync(2000, 500);
    }

    [Test]
    public async Task SpecifyingAShutdownMechanismDuringStopOverridesConfiguredShutdown()
    {
        var deactivationTime = 500L;

        for (var key = 0; key < 500; key++)
        {
            await Client.ActorFactory.CreateProxy<ISlowDeactivateActor>(key).Ping();
        }

        var stopwatch = new Orbit.Util.Time.Stopwatch();
        stopwatch.Start();
        await DisconnectClient(Client, new AddressableDeactivator.TimeSpan(
            new AddressableDeactivator.TimeSpan.Config(deactivationTime)
        ));
        stopwatch.Stop();

        Assert.GreaterOrEqual(stopwatch.Elapsed, deactivationTime);
        Assert.Less(stopwatch.Elapsed, deactivationTime + 200);
    }

    [Test]
    public async Task SendingAMessageToADeactivatedActorDuringShutdownReroutesTheMessage()
    {
        // Only use custom client
        await DisconnectClient(this.Client);

        var client = await StartClient(addressableTtl: TimeSpan.FromMilliseconds(300));

        var clientId = client.NodeId;

        for (var key = 0; key < 5; key++)
        {
            var cId = await client.ActorFactory.CreateProxy<IClientAwareActor>(key.ToString()).GetClient();
            Assert.AreEqual(clientId, cId);
        }

        var client2 = await StartClient();
        var client2Id = client2.NodeId;

        var job = client.Stop(
            new AddressableDeactivator.RateLimited(new AddressableDeactivator.RateLimited.Config(10)));

        await Task.Delay(150);

        var deactivated = TrackingGlobals.DeactivatedActors.First();
        var actor = client2.ActorFactory.CreateProxy<IClientAwareActor>(deactivated.ToString());

        Assert.AreEqual(client2Id, await actor.GetClient());

        await job;
    }

    [Test]
    public async Task ThrowingOnDeactivationDoesntStopDeactivation()
    {
        await DisconnectClient(this.Client);
        await DisconnectServers();

        await StartServer(addressableLeaseDurationSeconds: 1);
        var client = await StartClient(addressableTtl: TimeSpan.FromMilliseconds(100));
        var actor = client.ActorFactory.CreateProxy<IThrowsOnDeactivateActor>();
        await actor.Ping();

        await DisconnectClient(client);
        await Task.Delay(2000);

        Assert.AreEqual(1, TrackingGlobals.DeactivateTestCounts);
    }
}