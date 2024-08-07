namespace Orbit.Client.Stream;

public static class AsyncObservableExtensions
{
    public static readonly Action<Exception> Throw = ex => { };

    public static async Task<IAsyncSubscription<T>> Subscribe<T>(this IAsyncObservable<T> source, Action<T> onNext)
    {
        return await source.Subscribe(new AsyncSubscribe<T>(onNext, Throw));
    }

    public static async Task<IAsyncSubscription<T>> Subscribe<T>(this IAsyncObservable<T> source, Action<T> onNext, Action<Exception> onError)
    {
        return await source.Subscribe(new AsyncSubscribe<T>(onNext, onError));
    }
}