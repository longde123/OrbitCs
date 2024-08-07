using Google.Protobuf.WellKnownTypes;
using NUnit.Framework;
using Orbit.Client.Actor;
using Orbit.Client.Execution;
using Orbit.Server;
using Orbit.Server.Mesh;
using Orbit.Server.Mesh.Local;
using Orbit.Server.Service;
using Orbit.Util.Di;
using Orbit.Util.Time;

namespace Orbit.Client.Test;

/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

public class BaseIntegrationTest
{
    private readonly List<OrbitClient> _clients = new();
    private readonly List<OrbitServer> _servers = new();
    protected OrbitClient Client;
    protected Clock Clock = new();


    public void AdvanceTime(Duration duration)
    {
        Clock.AdvanceTime((long)duration.ToTimeSpan().TotalMilliseconds);
    }

    [SetUp]
    public async Task BeforeTest()
    {
        await StartServer();
        Client = await StartClient();
    }

    [TearDown]
    public async Task AfterTest()
    {
        foreach (var client in _clients.ToList())
        {
            await DisconnectClient(client);
        }

        await DisconnectServers();
        LocalNodeDirectory.Clear();
        LocalAddressableDirectory.Clear();
        Meters.Clear();
        TrackingGlobals.Reset();
        Clock = new Clock();
    }

    public async Task<OrbitServer> StartServer(
        int port = 50056,
        long addressableLeaseDurationSeconds = 10,
        long nodeLeaseDurationSeconds = 600,
        TimeSpan? tickRate = null,
        Action<ComponentContainerRoot>? containerOverrides = null)
    {
        var tickRateDuration = Duration.FromTimeSpan(tickRate ?? TimeSpan.FromSeconds(1));
        containerOverrides ??= r => { };
        var server = new OrbitServer(new OrbitServerConfig
        {
            ServerInfo = new LocalServerInfo($"https://localhost:{port}", port),
            AddressableLeaseDuration = new LeaseDuration(addressableLeaseDurationSeconds),
            NodeLeaseDuration = new LeaseDuration(nodeLeaseDurationSeconds),
            TickRate = tickRateDuration,
            Clock = Clock,
            ContainerOverrides = containerOverrides
        });

        await server.Start();

        _servers.Add(server);
        return server;
    }

    internal async Task DisconnectServers()
    {
        foreach (var server in _servers.ToList())
        {
            await DisconnectServer(server);
        }
    }

    public async Task DisconnectServer(OrbitServer server)
    {
        if (server == null)
        {
            return;
        }

        await server.Stop();

        _servers.Remove(server);
    }

    public async Task<OrbitClient> StartClient(
        int port = 50056,
        string nameSpace = "test",
        List<string>? packages = null,
        bool platformExceptions = true,
        TimeSpan? addressableTtl = null,
        ExternallyConfigured<AddressableDeactivator>? addressableDeactivation = null)
    {
        if (packages == null)
        {
            packages = new List<string> { "Orbit.Client.Actor" };
        }

        var ttl = addressableTtl ?? TimeSpan.FromMinutes(1);


        addressableDeactivation ??= new AddressableDeactivator.Instant.Config();
        var client = new OrbitClient(new OrbitClientConfig
        {
            GrpcEndpoint = $"https://localhost:{port}",
            Namespace = nameSpace,
            Packages = packages,
            Clock = Clock,
            PlatformExceptions = platformExceptions,
            AddressableTtl = ttl,
            AddressableDeactivator = addressableDeactivation
        });

        await client.Start();
        _clients.Add(client);

        return client;
    }

    public async Task DisconnectClient(OrbitClient? client, AddressableDeactivator? deactivator = null)
    {
        if (client != null)
        {
            await client.Stop(deactivator);
            _clients.Remove(client);
        }
    }
}