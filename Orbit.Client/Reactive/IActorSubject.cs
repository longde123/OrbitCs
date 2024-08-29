using Orbit.Client.Actor;
using Orbit.Client.Addressable;

namespace Orbit.Client.Reactive;

[NonConcrete]
public interface IActorSubject<T> : IAsyncObserver<T>, IActorWithStringKey
{
 
    Task Subscribe(IAsyncObserver<T> observer);

   
    Task UnSubscribe(IAsyncObserver<T> observer);
}