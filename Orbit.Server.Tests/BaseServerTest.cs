/*
Converted from Kotlin to C#
*/

using Google.Protobuf.WellKnownTypes;
using NUnit.Framework;
using Orbit.Server.Mesh;
using Orbit.Server.Mesh.Local;
using Orbit.Shared.Net;
using Orbit.Util.Di;
using Orbit.Util.Time;

namespace Orbit.Server;

public class BaseServerTest
{
    private readonly List<TestClient> _clients = new();
    private readonly List<OrbitServer> _servers = new();
    protected Clock Clock = new();


    public void AdvanceTime(TimeSpan duration)
    {
        Clock.AdvanceTime((long)duration.TotalMilliseconds);
    }

    [TearDown]
    public async Task AfterTest()
    {
        foreach (var client in _clients.ToList())
        {
            await DisconnectClient(client);
        }

        await Task.Delay(1);
        foreach (var server in _servers.ToList())
        {
            await DisconnectServer(server);
        }

        LocalNodeDirectory.Clear();
        LocalAddressableDirectory.Clear();
        Clock = new Clock();
    }

    public async Task<OrbitServer> StartServer(int port = 50056, long addressableLeaseDurationSeconds = 5,
        long nodeLeaseDurationSeconds = 10, int tickRate = 100,
        Action<ComponentContainerRoot>? containerOverrides = null)
    {
        if (containerOverrides == null)
        {
            containerOverrides = r => { };
        }

        var orbitServerConfig = new OrbitServerConfig();

        orbitServerConfig.ServerInfo = new LocalServerInfo
        {
            Port = port,
            Url = $"https://localhost:{port}"
        };
        orbitServerConfig.AddressableLeaseDuration = new LeaseDuration(addressableLeaseDurationSeconds);
        orbitServerConfig.NodeLeaseDuration = new LeaseDuration(nodeLeaseDurationSeconds);
        orbitServerConfig.Clock = Clock;
        orbitServerConfig.ContainerOverrides = containerOverrides;
        orbitServerConfig.TickRate = Duration.FromTimeSpan(TimeSpan.FromMilliseconds(tickRate));
        var server = new OrbitServer(orbitServerConfig);
        await Task.Delay(TimeSpan.FromSeconds(1));
        await server.Start();
        _servers.Add(server);
        return server;
    }

    public async Task DisconnectServer(OrbitServer? server)
    {
        if (server == null)
        {
            return;
        }

        await server.Stop();
        _servers.Remove(server);
    }

    public async Task<TestClient> StartClient(Action<Message> onReceive = null)
    {
        var client = new TestClient(onReceive);
        await client.Connect();

        _clients.Add(client);

        return client;
    }

    public async Task DisconnectClient(TestClient client)
    {
        await client.Disconnect();
        _clients.Remove(client);
    }
}