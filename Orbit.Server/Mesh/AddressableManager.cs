using Orbit.Server.Service;
using Orbit.Shared.Addressable;
using Orbit.Shared.Mesh;
using Orbit.Shared.OrException;
using Orbit.Util.Misc;
using Orbit.Util.Time;

namespace Orbit.Server.Mesh;

public class AddressableManager
{
    private readonly IAddressableDirectory _addressableDirectory;
    private readonly Clock _clock;
    private readonly ClusterManager _clusterManager;
    private readonly LeaseDuration _leaseExpiration;
    private readonly Meters.MeterTimer _placementTimer;

    public AddressableManager(IAddressableDirectory addressableDirectory, ClusterManager clusterManager, Clock clock,
        OrbitServerConfig config)
    {
        this._addressableDirectory = addressableDirectory;
        this._clusterManager = clusterManager;
        this._clock = clock;
        _leaseExpiration = config.AddressableLeaseDuration;

        _placementTimer = Meters.Timer(Meters.Names.PlacementTimer);
    }

    public async Task<NodeId> LocateOrPlace(string nameSpace, AddressableReference addressableReference)
    {
        return await LocateOrPlace(nameSpace, addressableReference,
            new List<NodeId>());
    }

    public async Task<NodeId> LocateOrPlace(string nameSpace, AddressableReference addressableReference,
        List<NodeId> ineligibleNodes)
    {
        //ineligibleNodes 无资格的；不合格的
        var key = new NamespacedAddressableReference(nameSpace, addressableReference);

        var it = await _addressableDirectory.GetOrPut(key, () => CreateNewEntry(key, ineligibleNodes));


        var invalidNode = await _clusterManager.GetNode(it.NodeId) == null || ineligibleNodes.Contains(it.NodeId);
        var expired = _clock.InPast(it.ExpiresAt.ToDateTime());
        if (expired || invalidNode)
        {
            var newEntry = await CreateNewEntry(key, ineligibleNodes.Concat(new List<NodeId> { it.NodeId }).ToList());
            if (await _addressableDirectory.CompareAndSet(key, it, newEntry))
            {
                return newEntry.NodeId;
            }

            return await LocateOrPlace(nameSpace, addressableReference,
                ineligibleNodes.Concat(new List<NodeId> { it.NodeId }).ToList());
        }

        return it.NodeId;
    }

    public async Task<AddressableLease> RenewLease(AddressableReference addressableReference, NodeId nodeId)
    {
        var key = new NamespacedAddressableReference(nodeId.Namespace, addressableReference);

        var it = await _addressableDirectory.Manipulate(key, initialValue =>
        {
            if (initialValue == null || !initialValue.NodeId.Equals(nodeId) ||
                _clock.InPast(initialValue.ExpiresAt.ToDateTime()))
            {
                throw new PlacementFailedException($"Could not renew lease for {addressableReference}");
            }

            initialValue.ExpiresAt = _clock.Now().Add(_leaseExpiration.ExpiresIn).ToTimestamp();
            initialValue.RenewAt = _clock.Now().Add(_leaseExpiration.RenewIn).ToTimestamp();

            return initialValue;
        });
        return it;
    }

    public async Task<bool> AbandonLease(AddressableReference addressableReference, NodeId nodeId)
    {
        var key = new NamespacedAddressableReference(nodeId.Namespace, addressableReference);
        var currentLease = await _addressableDirectory.Get(key);
        if (currentLease != null && currentLease.NodeId.Equals(nodeId) &&
            _clock.InFuture(currentLease.ExpiresAt.ToDateTime()))
        {
            return await _addressableDirectory.CompareAndSet(key, currentLease, null);
        }

        return false;
    }

    private async Task<AddressableLease> CreateNewEntry(NamespacedAddressableReference reference,
        List<NodeId> invalidNodes)
    {
        var nodeId = await Place(reference, invalidNodes);
        return new AddressableLease
        {
            NodeId = nodeId,
            Reference = reference.AddressableReference,
            ExpiresAt = _clock.Now().Add(_leaseExpiration.ExpiresIn).ToTimestamp(),
            RenewAt = _clock.Now().Add(_leaseExpiration.RenewIn).ToTimestamp()
        };
    }

    private async Task<NodeId> Place(NamespacedAddressableReference reference, List<NodeId> invalidNodes)
    {
        var v = await _placementTimer.Record(async () =>
        {
            var it = await AttemptUtil.Attempt(
                5,
                1000,
                long.MaxValue
                , 1.0f
                , null
                , () =>
                {
                    var allNodes = _clusterManager.GetAllNodes();
                    var potentialNodes = allNodes
                        .Where(node => !invalidNodes.Contains(node.Id))
                        .Where(node => node.Id.Namespace == reference.Namespace)
                        .Where(node => node.NodeStatus == NodeStatus.Active)
                        .Where(node => node.Capabilities.AddressableTypes.Contains(reference.AddressableReference.Type))
                        .ToList();
                    var r = new Random();

                    if (potentialNodes.Count() == 0)
                    {
                        throw new Exception("potentialNodes.Count() == 0");
                    }

                    var someRandomNumber = r.Next(0, potentialNodes.Count());
                    return potentialNodes[someRandomNumber].Id;
                }
            );
            return it;
        });

        return v;
    }
}