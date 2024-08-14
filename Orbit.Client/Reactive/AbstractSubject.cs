namespace Orbit.Client.Reactive;

public class AbstractSubject : IAsyncObserver
{
    public long MessageId { get; set; }
}