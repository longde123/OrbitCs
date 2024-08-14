using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Orbit.Server.Mesh;
using Orbit.Server.Mesh.Local;
using Orbit.Shared.Addressable;
using Orbit.Shared.Mesh;
using Orbit.Shared.Proto;
using Orbit.Util.Time;

namespace Orbit.Server.Tests;

public class AddressableManagerTests
{
    private static readonly Clock Clock = new();
    private readonly IAddressableDirectory _addressableDirectory = new LocalAddressableDirectory(Clock);
    private readonly INodeDirectory _nodeDirectory = new LocalNodeDirectory(Clock);
    private AddressableManager _addressableManager;
    private ClusterManager _clusterManager;


    private async Task<NodeInfo> Join(string nameSpace = "test", string addressableType = "testActor")
    {
        var custer = await _clusterManager.JoinCluster(
            nameSpace,
            new NodeCapabilities(new HashSet<string>
            {
                addressableType
            }),
            null,
            NodeStatus.Active);

        return custer;
    }

    [Test]
    public async Task AddressableIsPlacedOnAvailableNode()
    {
        var address = new AddressableReference("testActor", new Key.StringKey("a"));
        var node = await Join();
        var placedNode = await _addressableManager.LocateOrPlace("test", address);

        Assert.AreEqual(node.Id, placedNode);
    }

    [Test]
    public async Task AddressableShouldBePlacedOnlyOnNodeInNamespace()
    {
        var address = new AddressableReference("testActor", new Key.StringKey("a"));
        var nodes = new Dictionary<int, NodeInfo>
        {
            {
                1, await Join()
            },
            {
                2, await Join("test2")
            }
        };

        foreach (var _ in Enumerable.Range(0, 100))
        {
            var placedNode = await _addressableManager.LocateOrPlace("test", address);
            Assert.AreEqual(nodes[1].Id, placedNode);
        }
    }

    [Test]
    public async Task AddressableShouldBePlacedOnlyOnNodeWithMatchingCapability()
    {
        var address = new AddressableReference("testActor", new Key.StringKey("a"));
        var nodes = new Dictionary<int, NodeInfo>
        {
            {
                1, await Join(addressableType: "testActor")
            },
            {
                2, await Join(addressableType: "invalidActor")
            }
        };

        foreach (var _ in Enumerable.Range(0, 100))
        {
            var placedNode = await _addressableManager.LocateOrPlace("test", address);
            Assert.AreEqual(nodes[1].Id, placedNode);
        }
    }

    [Test]
    public async Task AddressableShouldNotBePlacedOnRemovedNode()
    {
        var address = new AddressableReference("testActor", new Key.StringKey("a"));
        var nodes = new Dictionary<int, NodeInfo>
        {
            {
                1, await Join()
            },
            {
                2, await Join()
            }
        };

        await _nodeDirectory.Remove(nodes[1].Id);
        await _clusterManager.Tick();

        foreach (var _ in Enumerable.Range(0, 100))
        {
            var placedNode = await _addressableManager.LocateOrPlace("test", address);
            Assert.AreEqual(nodes[2].Id, placedNode);
        }
    }

    [Test]
    public async Task AddressableShouldNotBePlacedOnADrainingNode()
    {
        var address = new AddressableReference("testActor", new Key.StringKey("a"));
        var nodes = new Dictionary<int, NodeInfo>
        {
            {
                1, await Join()
            },
            {
                2, await Join()
            }
        };

        await _clusterManager.UpdateNode(nodes[1].Id, node =>
        {
            node.NodeStatus = NodeStatus.Draining;
            return node;
        });

        foreach (var _ in Enumerable.Range(0, 100))
        {
            var placedNode = await _addressableManager.LocateOrPlace("test", address);
            Assert.AreEqual(nodes[2].Id, placedNode);
        }
    }

    [Test]
    public async Task AddressableShouldBeReplacedFromExpiredNode()
    {
        var address = new AddressableReference("testActor", new Key.StringKey("a"));
        var nodes = new Dictionary<int, NodeInfo>
        {
            {
                1, await Join()
            },
            {
                2, await Join()
            }
        };

        await _clusterManager.UpdateNode(nodes[1].Id, node =>
        {
            node.Lease = NodeLeaseExtensions.Expired;
            return node;
        });

        foreach (var _ in Enumerable.Range(0, 100))
        {
            var placedNode = await _addressableManager.LocateOrPlace("test", address);
            Assert.AreEqual(nodes[2].Id, placedNode);
        }
    }

    [Test]
    public async Task IneligibleNodesAreNotIncludedInPlacement()
    {
        var address = new AddressableReference("testActor", new Key.StringKey("a"));
        var nodes = new Dictionary<int, NodeInfo>
        {
            {
                1, await Join()
            },
            {
                2, await Join()
            },
            {
                3, await Join()
            }
        };

        foreach (var _ in Enumerable.Range(0, 100))
        {
            var placedNode = await _addressableManager
                .LocateOrPlace("test", address, new List<NodeId>
                {
                    nodes[2].Id,
                    nodes[3].Id
                });
            Assert.AreEqual(nodes[1].Id, placedNode);
        }
    }

    [Test]
    public async Task AbandoningAnAddressableLeaseShouldRemoveItFromAddressableDirectory()
    {
        var address = new AddressableReference("testActor", new Key.StringKey("a"));
        var node = await Join();
        await _addressableManager.LocateOrPlace("test", address);
        Assert.NotNull(await _addressableDirectory.Get(new NamespacedAddressableReference("test", address)));

        Assert.True(await _addressableManager.AbandonLease(address, node.Id));
        Assert.Null(await _addressableDirectory.Get(new NamespacedAddressableReference("test", address)));
    }

    [Test]
    public async Task AbandoningAnAddressableLeaseNotAssignedToNodeIsIgnored()
    {
        var address = new AddressableReference("testActor", new Key.StringKey("a"));
        await Join();
        await _addressableManager.LocateOrPlace("test", address);
        var node2 = await Join();
        Assert.False(await _addressableManager.AbandonLease(address, node2.Id));
    }

    [Test]
    public async Task AbandoningANonExistantAddressableLeaseIsIgnored()
    {
        var address = new AddressableReference("testActor", new Key.StringKey("a"));
        Assert.False(await _addressableManager.AbandonLease(address, new NodeId("node", "test")));
    }

    [Test]
    public async Task AbandoningAnExpiredLeaseDoesntRemoveFromDirectory()
    {
        var address = new AddressableReference("testActor", new Key.StringKey("a"));
        await Join();
        await _addressableManager.LocateOrPlace("test", address);
        var reference = new NamespacedAddressableReference("test", address);
        var lease = await _addressableDirectory.Get(reference);
        var newlease = lease.ToAddressableLeaseProto().ToAddressableLease();
        newlease.ExpiresAt = new Timestamp(0, 0);
        await _addressableDirectory.CompareAndSet(reference, lease, newlease);

        Assert.False(await _addressableManager.AbandonLease(address, new NodeId("node", "test")));
        Assert.NotNull(_addressableDirectory.Get(reference));
    }

    [SetUp]
    public void Initialize()
    {
        long nodeLeaseDurationSeconds = 10;
        var config = new OrbitServerConfig
        {
            NodeLeaseDuration = new LeaseDuration(nodeLeaseDurationSeconds)
        };
        Clock.ResetToNow();
        LocalNodeDirectory.Clear();
        LocalAddressableDirectory.Clear();
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Trace); // 设置最低日志级别
        });
        _clusterManager = new ClusterManager(config, Clock, _nodeDirectory, loggerFactory);
        _addressableManager = new AddressableManager(
            _addressableDirectory,
            _clusterManager,
            Clock,
            config, loggerFactory);
    }
}