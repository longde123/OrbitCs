namespace Orbit.Client.Stream;

public class AsyncSubscription<T> : IAsyncSubscription<T>
{
    private AsyncSubject<T> parent;
    private IAsyncObserver<T> unsubscribeTarget;

    public AsyncSubscription(AsyncSubject<T> asyncSubject, IAsyncObserver<T> observer)
    {
        parent = asyncSubject;
        unsubscribeTarget = observer;
    }

    public async Task Dispose()
    {
        var listObserver = parent.outObserver;
        await listObserver.Remove(unsubscribeTarget);

        unsubscribeTarget = null;
        parent = null;
    }
}