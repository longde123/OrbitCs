namespace Orbit.Util.Time;

public class Stopwatch
{
    private readonly long _startTime;
    private Clock _clock;

    public Stopwatch()
    {
    }

    public Stopwatch(Clock clock)
    {
        _clock = clock;
        _startTime = clock.CurrentTime;
    }

    public TimeMs Elapsed => _clock.CurrentTime - _startTime;

    public void Start()
    {
        _clock = new Clock();
    }

    public void Stop()
    {
    }

    public static Stopwatch Start(Clock clock)
    {
        return new Stopwatch(clock);
    }
}

public struct ElapsedAndResult<T>
{
    public TimeMs Elapsed { get; }
    public T Result { get; }

    public ElapsedAndResult(TimeMs elapsed, T result)
    {
        Elapsed = elapsed;
        Result = result;
    }
}