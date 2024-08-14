using System.Collections.Concurrent;
using Orbit.Shared.Mesh;
using Orbit.Util.Concurrent;
using Orbit.Util.Di;
using Orbit.Util.Time;

namespace Orbit.Server.Mesh.Local;

public class LocalNodeDirectory : HashMapBackedAsyncMap<NodeId, NodeInfo>, INodeDirectory
{
    public static ConcurrentDictionary<NodeId, NodeInfo> GlobalMap = new();
    private readonly Clock _clock;

    public LocalNodeDirectory(Clock clock)
    {
        _clock = clock;
    }

    public override ConcurrentDictionary<NodeId, NodeInfo> Map => GlobalMap;

    public async Task Tick()
    {
        var toDelete = GlobalMap.Values.Where(nodeInfo => _clock.InPast(nodeInfo.Lease.ExpiresAt.ToDateTime())).ToList();
        foreach (var nodeInfo in toDelete)
        {
            await Remove(nodeInfo.Id);
        }
    }

    public async Task<IEnumerable<(NodeId, NodeInfo)>> Entries()
    {
        return GlobalMap.Select(kv => (kv.Key, kv.Value));
    }

    public async Task<long> Count()
    {
        return GlobalMap.Count;
    }

    public async Task<bool> IsHealthy()
    {
        return true;
    }

    public static void Clear()
    {
        GlobalMap.Clear();
    }

    public class LocalNodeDirectorySingleton : ExternallyConfigured<INodeDirectory>
    {
        public override Type InstanceType => typeof(LocalNodeDirectory);
    }
}