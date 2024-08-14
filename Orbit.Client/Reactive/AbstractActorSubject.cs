using Orbit.Client.Actor;
using Orbit.Client.Addressable;

namespace Orbit.Client.Reactive;

public class AbstractActorSubject<T> : AbstractActor, IActorSubject<T>
{
    private AsyncListObserver<T> _receive;

    public AbstractActorSubject()
    {
        _receive = new AsyncListObserver<T>();
    }

    public async Task OnError(Exception error)
    {
        await _receive.OnError(error);
    }

    public async Task OnNext(T value)
    {
        await _receive.OnNext(value);
    }


    //IActorStream
    public async Task Subscribe(IAsyncObserver<T> observer)
    {
        await _receive.Add(observer);
    }


    public async Task UnSubscribe(IAsyncObserver<T> observer)
    {
        await _receive.Remove(observer);
    }
}