using Orbit.Client.Addressable;

namespace Orbit.Client.Stream;

[NonConcrete]
public interface IAsyncSubject<TSource, TResult> : IAsyncObserver<TSource>, IAsyncObservable<TResult>
{
}

[NonConcrete]
public interface IAsyncSubject<T> : IAsyncSubject<T, T>, IAsyncObserver<T>, IAsyncObservable<T>
{
}