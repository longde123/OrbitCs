using Orbit.Client.Addressable;

namespace Orbit.Client.Stream;

[NonConcrete]
public interface IAsyncObservable<T>
{
    Task<IAsyncSubscription<T>> Subscribe(IAsyncObserver<T> observer);
}