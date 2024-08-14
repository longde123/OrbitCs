using Orbit.Client.Actor;
using Orbit.Client.Addressable;

namespace Orbit.Client.Reactive;

[NonConcrete]
public interface IActorSubject<T> : IAsyncObserver<T>, IActorWithStringKey
{
    [OneWay]
    Task Subscribe(IAsyncObserver<T> observer);

    [UnOneWay]
    Task UnSubscribe(IAsyncObserver<T> observer);
}