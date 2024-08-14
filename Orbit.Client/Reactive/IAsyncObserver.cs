using Orbit.Client.Addressable;

namespace Orbit.Client.Reactive;

[NonConcrete]
public interface IAsyncObserver
{
}

[NonConcrete]
public interface IAsyncObserver<T> : IAsyncObserver
{
    Task OnError(Exception error);
    Task OnNext(T value);
}