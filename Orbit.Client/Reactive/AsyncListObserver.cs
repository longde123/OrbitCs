using System.Collections.Immutable;

namespace Orbit.Client.Reactive;

public class AsyncListObserver<T> : IAsyncObserver<T>
{
    private readonly List<IAsyncObserver<T>> _observers;

    public AsyncListObserver()
    {
        _observers = new List<IAsyncObserver<T>>();
    }


    public async Task OnError(Exception error)
    {
        var targetObservers = _observers;
        for (var i = 0; i < targetObservers.Count; i++)
        {
            await targetObservers[i].OnError(error);
        }
    }

    public async Task OnNext(T value)
    {
        var targetObservers = _observers;
        for (var i = 0; i < targetObservers.Count; i++)
        {
            await targetObservers[i].OnNext(value);
        }
    }

    public async Task Add(IAsyncObserver<T> observer)
    {
        _observers.Add(observer);
    }

    public async Task Remove(IAsyncObserver<T> observer)
    {
        _observers.Remove(observer);
    }
}