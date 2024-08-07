using Orbit.Util.Concurrent;

namespace Orbit.Server.Concurrent;

public class RuntimeScopes
{
    private readonly Action<AggregateException> _exceptionHandler;

    public RuntimeScopes(Action<AggregateException> exceptionHandler)
    {
        _exceptionHandler = exceptionHandler;
        CpuScope = new SupervisorScope(_exceptionHandler);
        IoScope = new SupervisorScope(_exceptionHandler);
    }

    public SupervisorScope CpuScope { get; }
    public SupervisorScope IoScope { get; }
}