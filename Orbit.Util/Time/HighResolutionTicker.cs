namespace Orbit.Util.Time;
// C# code translated from Kotlin



public static class TickerUtils
{
    public static async IAsyncEnumerable<TUnit> HighResolutionTicker<TUnit>(double ticksPerSecond,
        CancellationToken cancellationToken)
    {
        //1 second = 1,000 milliseconds = 1,000,000 microseconds = 1,000,000,000 nanoseconds
        var rate = (long)(1000000 / ticksPerSecond);
        var time = () => DateTime.Now.Ticks / TimeSpan.TicksPerMicrosecond;
        var count = 0;
        var startTime = time();
        while (!cancellationToken.IsCancellationRequested)
        {
            // long nextDelay = (startTime + (rate * (++count))-time());  
            // await Task.Delay(TimeSpan.FromMicroseconds(nextDelay));  
            yield return await  Task.Run(async () =>
            {
                var nextDelay = startTime + rate * ++count; 
                while (nextDelay > time())
                { 
                    Thread.Sleep(0);
                    cancellationToken.ThrowIfCancellationRequested();
                }
                return default(TUnit);
            },cancellationToken); 
        }
    }
}