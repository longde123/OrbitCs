namespace Orbit.Util.Time;

public class Clock
{
    private long _offsetTime;

    public long CurrentTime => ClockUtils.CurrentTimeMillis() + _offsetTime;

    public DateTime Now()
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(CurrentTime).DateTime;
    }

    public void AdvanceTime(long offset)
    {
        _offsetTime += offset;
    }

    public void ResetToNow()
    {
        _offsetTime = 0;
    }

    public bool InFuture(DateTime time)
    {
        var now = Now();
        return time > now;
    }

    public bool InPast(DateTime time)
    {
        return !InFuture(time);
    }

    public bool NowOrPast(DateTime time)
    {
        return time == Now() || InPast(time);
    }

    public TimeSpan Until(DateTime time)
    {
        return time - Now();
    }

    public override string ToString()
    {
        return Now().ToUniversalTime().ToString();
    }
}

public static class ClockUtils
{
    public static long CurrentTimeMillis()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}

public class TimeMs
{
    public TimeMs(long value)
    {
        Value = value;
    }

    public long Value { get; }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator TimeMs(long value)
    {
        return new TimeMs(value);
    }

    public static implicit operator long(TimeMs timeMs)
    {
        return timeMs.Value;
    }
}