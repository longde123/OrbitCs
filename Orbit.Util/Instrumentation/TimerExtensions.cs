namespace Orbit.Util.Instrumentation;

public static class TimerExtensions
{
    public static async Task<TR> RecordSuspended<TR>(this Timer timer, Func<Task<TR>> func)
    {
        // Timer.Sample sample = null;
        //   sample = timer.Measure();
        return await func();
    }
}