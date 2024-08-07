namespace Orbit.Client.Stream;

public class AsyncSubscribe<T> : IAsyncObserver<T>
{
    private readonly Action<T> onNext;
    private readonly Action<Exception> onError;

    private int isStopped = 0;

    public AsyncSubscribe(Action<T> onNext, Action<Exception> onError)
    {
        this.onNext = onNext;
        this.onError = onError;
    }

    public async Task OnNext(T value)
    {
        if (isStopped == 0)
        {
            onNext(value);
        }
    }

    public async Task OnError(Exception error)
    {
        if (Interlocked.Increment(ref isStopped) == 1)
        {
            onError(error);
        }
    }
}