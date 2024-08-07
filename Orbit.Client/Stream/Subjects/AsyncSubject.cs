namespace Orbit.Client.Stream;

public class AsyncSubject<T> : IAsyncSubject<T>
{
    private Exception lastError;
    public AsyncListObserver<T> outObserver;

    public async Task OnError(Exception error)
    {
        if (error == null) throw new ArgumentNullException("error");

        await outObserver.OnError(error);
    }

    public async Task OnNext(T value)
    {
        await outObserver.OnNext(value);
    }

    public async Task<IAsyncSubscription<T>> Subscribe(IAsyncObserver<T> observer)
    {
        if (observer == null) throw new ArgumentNullException("observer");

        await outObserver.Add(observer);

        return new AsyncSubscription<T>(this, observer);
    }
}