using System.Collections.Concurrent;
using Orbit.Shared.Addressable;
using Orbit.Util.Concurrent;
using Orbit.Util.Di;
using Orbit.Util.Time;

namespace Orbit.Server.Mesh.Local;

public class LocalAddressableDirectory : HashMapBackedAsyncMap<NamespacedAddressableReference, AddressableLease>,
    IAddressableDirectory
{
    public static ConcurrentDictionary<NamespacedAddressableReference, AddressableLease> GlobalMap = new();
    private readonly Clock _clock;

    public LocalAddressableDirectory(Clock clock)
    {
        this._clock = clock;
    }

    public override ConcurrentDictionary<NamespacedAddressableReference, AddressableLease> Map => GlobalMap;

    public Task<bool> IsHealthy()
    {
        return Task.FromResult(true);
    }

    public async Task Tick()
    {
        // Cull expired
        var toDelete = GlobalMap.Values.Where(value => _clock.InPast(value.ExpiresAt.ToDateTime())).ToList();
        foreach (var item in toDelete)
        {
            Remove(new NamespacedAddressableReference(item.NodeId.Namespace, item.Reference));
        }
    }

    public override Task<long> Count()
    {
        return Task.FromResult((long)GlobalMap.Count);
    }

    public static void Clear()
    {
        GlobalMap.Clear();
    }

    public class LocalAddressableDirectorySingleton : ExternallyConfigured<IAddressableDirectory>
    {
        public override Type InstanceType => typeof(LocalAddressableDirectory);
    }
}