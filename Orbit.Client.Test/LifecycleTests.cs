using Google.Protobuf.WellKnownTypes;
using NUnit.Framework;
using Orbit.Client.Actor;
using Orbit.Client.Net;
using Orbit.Server.Service;

namespace Orbit.Client.Test;
// This code is for a testing framework in C# using NUnit and asynchronous programming.

public class LifecycleTests : BaseIntegrationTest
{
    [Test]
    public async Task RunningClientIsInStartedState()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
        Assert.AreEqual(ClientState.Connected, Client.Status);
    }

    [Test]
    public async Task StoppingClientPutsItIntoIdleState()
    {
        await DisconnectClient(Client);
        Assert.AreEqual(ClientState.Idle, Client.Status);
    }

    [Test]
    public async Task ConnectingAClientAddsItToTheServerCluster()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
        Assert.AreEqual(1.0, Meters.ConnectedClients);
    }

    [Test]
    public async Task DisconnectingAClientRemovesItFromTheCluster()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
        Assert.AreEqual(1.0, Meters.ConnectedClients);

        await DisconnectClient(Client);

        await Task.Delay(TimeSpan.FromSeconds(5));
        Assert.AreEqual(0.0, Meters.ConnectedClients);
    }

    [Test]
    public async Task DisconnectingAClientSendsActorToNewClient()
    {
        var key = "test";
        var result1 = await Client.ActorFactory.CreateProxy<IDActor>(key).GetId();
        Assert.AreEqual(result1, key);
        await DisconnectClient(Client);

        await Task.Delay(TimeSpan.FromSeconds(5));
        Assert.AreEqual(0.0, Meters.ConnectedClients);

        // Assure addressable lease expires
        AdvanceTime(Duration.FromTimeSpan(TimeSpan.FromSeconds(10)));

        var client2 = await StartClient();
        var result2 = await client2.ActorFactory.CreateProxy<IDActor>(key).GetId();
        Assert.AreEqual(result2, key);
    }
}