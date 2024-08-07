namespace Orbit.Util.Time;

public class Timestamp : IEquatable<Timestamp>, IComparable<Timestamp>
{
    public Timestamp()
    {
    }

    public Timestamp(long seconds, int nanos)
    {
        Seconds = seconds;
        Nanos = nanos;
    }

    public long Seconds { get; set; }
    public int Nanos { get; set; }

    public int CompareTo(Timestamp other)
    {
        return Compare(this, other);
    }

    public bool Equals(Timestamp? other)
    {
        return CompareTo(other) == 0;
    }

    public override bool Equals(object? obj)
    {
        if (obj != null && obj is Timestamp nid)
        {
            return Equals(nid);
        }

        return false;
    }

    public static int Compare(Timestamp first, Timestamp second)
    {
        var secondCompare = first.Seconds.CompareTo(second.Seconds);
        if (secondCompare != 0)
        {
            return secondCompare;
        }

        return first.Nanos.CompareTo(second.Nanos);
    }

    public static bool operator ==(Timestamp first, Timestamp second)
    {
        return first.Equals(second);
    }

    public static bool operator !=(Timestamp first, Timestamp second)
    {
        return !first.Equals(second);
    }

    public static bool operator <(Timestamp first, Timestamp second)
    {
        return Compare(first, second) < 0;
    }

    public static bool operator >(Timestamp first, Timestamp second)
    {
        return Compare(first, second) > 0;
    }

    public static bool operator <=(Timestamp first, Timestamp second)
    {
        return Compare(first, second) <= 0;
    }

    public static bool operator >=(Timestamp first, Timestamp second)
    {
        return Compare(first, second) >= 0;
    }

    public bool IsAfter(DateTime time)
    {
        return CompareTo(time.ToTimestamp()) > 0;
    }

    public bool IsExactly(DateTime time)
    {
        return CompareTo(time.ToTimestamp()) == 0;
    }

    public static Timestamp Now()
    {
        return DateTime.Now.ToTimestamp();
    }

    public DateTime ToDateTime()
    {
        return new DateTime(1970, 1, 1).AddSeconds(Seconds).AddMilliseconds(Nanos / 1000000f);
    }

    public static Timestamp FromDateTime(DateTime dateTime)
    {
        var millis = (long)(dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
        return new Timestamp(millis / 1000, (int)(millis % 1000 * 1000000));
    }
}

public static class TimestampExtensions
{
    public static Timestamp ToTimestamp(this DateTime dateTime)
    {
        var millis = (long)(dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
        return new Timestamp(millis / 1000, (int)(millis % 1000 * 1000000));
    }
}