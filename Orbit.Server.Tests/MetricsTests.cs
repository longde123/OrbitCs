using NUnit.Framework;
using Orbit.Server.Mesh;
using Orbit.Server.Service;
using Orbit.Shared.Mesh;

namespace Orbit.Server.Tests;

public class MetricsTests : BaseServerTest
{
    [Test]
    public async Task ConnectingToServiceIncrementsConnectedMetric()
    {
        await StartServer();

        await StartClient();
        await StartClient();

        await Task.Delay(TimeSpan.FromSeconds(5));
        Assert.That(Meters.ConnectedClients, Is.EqualTo(2.0), "Connected clients count is not 2");
    }

    [Test]
    public async Task DisconnectingFromServiceDecrementsConnectedMetric()
    {
        await StartServer();
        var client = await StartClient();
        await Task.Delay(TimeSpan.FromSeconds(1));
        Assert.That(Meters.ConnectedClients, Is.EqualTo(1.0), "Connected clients count is not 1");

        await DisconnectClient(client);
        await Task.Delay(TimeSpan.FromSeconds(1));
        Assert.That(Meters.ConnectedClients, Is.EqualTo(0.0), "Connected clients count is not 0");
    }

    [Test]
    public async Task SendingMessagesToAnAddressableIncrementsPlacements()
    {
        var server = await StartServer(tickRate: 1);
        var spy = server.Container.Resolve<ClusterManager>();
        spy.GetAllNodes().Where(node => node.NodeStatus == NodeStatus.Active);

        await Task.Delay(TimeSpan.FromSeconds(3));
        var client = await StartClient();
        await client.SendMessage("test message", "address 1");
        await client.SendMessage("test message", "address 2");
        await Task.Delay(TimeSpan.FromSeconds(3));
        Assert.That(Meters.PlacementTimerCount, Is.EqualTo(2.0), "Placement count is not 2");
        Assert.That(Meters.PlacementTimerTotalTime, Is.GreaterThan(0.0), "Placement total time is not greater than 0");
    }

    [Test]
    public async Task SendingAMessageToAddressablesIncrementsTotalAddressables()
    {
        await StartServer();

        var client = await StartClient();
        await client.SendMessage("test message", "address 1");
        await client.SendMessage("test message", "address 2");
        await Task.Delay(TimeSpan.FromSeconds(5));
        Assert.That(Meters.AddressableCount, Is.EqualTo(2.0), "Addressable count is not 2");
    }

    [Test]
    public async Task ExpiringAnAddressableDecrementsTotalAddressables()
    {
        var server = await StartServer(addressableLeaseDurationSeconds: 1);

        var client = await StartClient();
        await client.SendMessage("test message", "address 1");
        await server.Tick();
        await Task.Delay(TimeSpan.FromSeconds(1));
        Assert.That(Meters.AddressableCount, Is.EqualTo(1.0), "Addressable count is not 1");

        await Task.Delay(TimeSpan.FromSeconds(1));
        await server.Tick();
        Assert.That(Meters.AddressableCount, Is.EqualTo(0.0), "Addressable count is not 0");
    }

    [Test]
    public async Task AddingNodesIncrementsNodeCount()
    {
        await StartServer();

        Assert.That(Meters.NodeCount, Is.EqualTo(1.0), "Node count is not 1");

        await StartServer(50057);

        Assert.That(Meters.NodeCount, Is.EqualTo(2.0), "Node count is not 2");
    }

    [Test]
    public async Task ExpiredNodeLeaseDecrementsNodeCount()
    {
        var server = await StartServer(nodeLeaseDurationSeconds: 60);

        var secondServer = await StartServer(50057);
        await secondServer.Tick();
        Assert.That(Meters.NodeCount, Is.EqualTo(2.0), "Node count is not 2");

        await DisconnectServer(secondServer);
        await server.Tick();

        await Task.Delay(10000);

        await server.Tick();
        Assert.That(Meters.NodeCount, Is.EqualTo(1.0), "Node count is not 1");
    }

    [Test]
    public async Task ConnectingAndDisconnectingServerNodesAdjustsConnectedNodesCount()
    {
        var server = await StartServer();

        Assert.That(Meters.ConnectedNodes, Is.EqualTo(0.0), "Connected nodes count is not 0");

        var secondServer = await StartServer(50057, tickRate: 100000);
        await server.Tick();
        await secondServer.Tick();

        Assert.That(Meters.ConnectedNodes, Is.EqualTo(1.0), "Connected nodes count is not 1");

        await DisconnectServer(secondServer);
        await server.Tick();
        Assert.That(Meters.ConnectedNodes, Is.EqualTo(0.0), "Connected nodes count is not 0");
    }

    [Test]
    public async Task SendingMessagesAddsThePayloadSizeToTheTotal()
    {
        await StartServer();

        var client = await StartClient();
        await client.SendMessage("test", "address 1");
        await client.SendMessage("test 2", "address 1");
        await Task.Delay(TimeSpan.FromSeconds(3));
        Assert.That(Meters.MessageSizes, Is.EqualTo(136.0), "Message sizes is not 136");
    }

    [Test]
    public async Task SendingMessagesAddsToTheMessageCount()
    {
        await StartServer();

        var client = await StartClient();
        await client.SendMessage("test", "address 1");
        await client.SendMessage("test 2", "address 1");
        await Task.Delay(TimeSpan.FromSeconds(3));
        Assert.That(Meters.MessagesCount, Is.EqualTo(2.0), "Message count is not 2");
    }

    [Test]
    public async Task ConstantTickTimerGoingLongIncreasesSlowTickCount()
    {
        var pulse = TimeSpan.FromSeconds(2);
        var server = await StartServer(tickRate: 1);
        var spy = server.Container.Resolve<ClusterManager>();
        await spy.Tick();
        AdvanceTime(pulse);
        await Task.Delay(10);

        Assert.That(Meters.SlowTickCount, Is.GreaterThan(0.0), "Slow tick count is not greater than 0");
    }

    [Test]
    public async Task ConstantTickTimerElapsesAndRecordsTicks()
    {
        var server = await StartServer(tickRate: 1);
        var spy = server.Container.Resolve<ClusterManager>();
        await spy.Tick();
        AdvanceTime(TimeSpan.FromSeconds(5));
        Assert.That(Meters.TickTimerCount, Is.GreaterThan(3.0), "Tick timer count is not greater than 3");
        Assert.That(Meters.TickTimerTotal, Is.GreaterThanOrEqualTo(0.0),
            "Tick timer total is not greater than or equal to 0");
    }
}