using Orbit.Client.Addressable;

namespace Orbit.Client.Stream;

[NonConcrete]
public interface IAsyncObserver<T>
{
    Task OnError(Exception error);
    Task OnNext(T value);
}