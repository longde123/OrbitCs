using Orbit.Client.Actor;

namespace Orbit.Client.Stream;

public class ActorStream<T> :  IActorStream<T> 
{
    private IAsyncSubject<T> _receive;

    public ActorStream()
    {
        _receive = new AsyncSubject<T>();
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
    public async Task<IAsyncSubscription<T>> Subscribe(IAsyncObserver<T> observer)
    {
        return await _receive.Subscribe(observer);
    }

    public async Task<IAsyncSubscription<T>> Subscribe(Action<T> observer)
    {
        return await _receive.Subscribe(observer);
    }
}