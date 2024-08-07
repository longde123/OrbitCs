using Orbit.Client.Addressable;

namespace Orbit.Client.Stream;

[NonConcrete]
public interface IAsyncSubscription<T>
{
    Task Dispose();
}