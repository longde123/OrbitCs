namespace Orbit.Util.Concurrent;

public class SupervisorScope
{
    private readonly Action<AggregateException> _exceptionHandler;


    public SupervisorScope(Action<AggregateException> exceptionHandler)
    {
        this._exceptionHandler = exceptionHandler;
    }
}