using Orbit.Server.Service;
using Orbit.Shared.Addressable;
using Orbit.Util.Concurrent;

namespace Orbit.Server.Mesh;

public interface IAddressableDirectory : IAsyncMap<NamespacedAddressableReference, AddressableLease>, IHealthCheck
{
    Task Tick();
}