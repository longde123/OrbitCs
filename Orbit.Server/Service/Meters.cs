using System.Diagnostics.Metrics;
using Orbit.Util.Time;

namespace Orbit.Server.Service;

public class Meters
{
    public static IMeterNames Names = new MeterNames();
    private static Meter _globalMeter;
    private static Dictionary<string, AbstractMeter> _globalMeters;

    public static int ConnectedClients => GetMeter(Names.ConnectedClients).GetFirst();
    public static int PlacementTimerCount => GetMeter(Names.PlacementTimer, "count").GetFirst();
    public static int PlacementTimerTotalTime => GetMeter(Names.PlacementTimer, "total_time").GetSecond();
    public static int AddressableCount => GetMeter(Names.AddressableCount).GetFirst();
    public static int NodeCount => GetMeter(Names.NodeCount).GetFirst();
    public static int ConnectedNodes => GetMeter(Names.ConnectedNodes).GetFirst();
    public static int MessagesCount => GetMeter(Names.MessageSizes, "count").GetFirst();
    public static int MessageSizes => GetMeter(Names.MessageSizes, "total").GetSecond();
    public static int SlowTickCount => GetMeter(Names.SlowTicks).GetFirst();
    public static int TickTimerCount => GetMeter(Names.TickTimer, "count").GetFirst();
    public static int TickTimerTotal => GetMeter(Names.TickTimer, "total_time").GetSecond();

    public static void Init(IMeterRegistry meter)
    {
        if (_globalMeter != null)
        {
            _globalMeter.Dispose();
        }

        meter.Init();
        _globalMeter = new Meter(meter.GetName(), "1.0.0");
        _globalMeters = new Dictionary<string, AbstractMeter>();
    }

    public static void Clear()
    {
        //GlobalMeters.Clear(); 
        //GlobalMeter.Dispose();
    }

    private static AbstractMeter GetMeter(string name, string? statistic = null)
    {
        var key = name;

        if (_globalMeters.TryGetValue(key, out var meter))
        {
            return meter;
        }

        throw new Exception($" GetMeter name {name} statistic {statistic} is null");
    }

    public static MeterCounter Counter(string key)
    {
        if (_globalMeters.TryGetValue(key, out var meter))
        {
            return (MeterCounter)meter;
        }

        meter = new MeterCounter(_globalMeter.CreateCounter<int>(key));
        _globalMeters.Add(key, meter);
        return (MeterCounter)meter;
    }

    public static MeterSummary Summary(string key)
    {
        if (_globalMeters.TryGetValue(key, out var meter))
        {
            return (MeterSummary)meter;
        }

        meter = new MeterSummary(_globalMeter.CreateCounter<int>(key + "count"),
            _globalMeter.CreateHistogram<int>(key + "total"));
        _globalMeters.Add(key, meter);
        return (MeterSummary)meter;
    }

    public static MeterTimer Timer(string key)
    {
        if (_globalMeters.TryGetValue(key, out var meter))
        {
            return (MeterTimer)meter;
        }

        meter = new MeterTimer(_globalMeter.CreateCounter<int>(key + "count"),
            _globalMeter.CreateHistogram<int>(key + "total_time"));
        _globalMeters.Add(key, meter);
        return (MeterTimer)meter;
    }

    public static AbstractMeter Gauge(string key, Func<int> func)
    {
        if (_globalMeters.TryGetValue(key, out var meter))
        {
            return meter;
        }

        meter = new MeterGauge(_globalMeter.CreateObservableGauge(key, func), func);
        _globalMeters.Add(key, meter);
        return meter;
    }

    public class AbstractMeter
    {
        protected int First;
        public int Second;

        public AbstractMeter()
        {
            First = 0;
        }

        public virtual int GetFirst()
        {
            return First;
        }

        public virtual int GetSecond()
        {
            return Second;
        }
    }

    public class MeterTimer : AbstractMeter
    {
        private readonly Clock _clock;
        private readonly Histogram<int> _histogram;
        private readonly Counter<int> _meter;


        public MeterTimer(Counter<int> meter, Histogram<int> histogram)
        {
            _histogram = histogram;
            _meter = meter;
            _clock = new Clock();
        }

        public async Task<T> Record<T>(Func<Task<T>> func)
        {
            var stopwatch = Stopwatch.Start(_clock);
            var value = await func.Invoke();
            _meter.Add(1);
            First += 1;
            Second += (int)stopwatch.Elapsed.Value;
            _histogram.Record(Second);

            return value;
        }
    }

    public class MeterSummary : AbstractMeter
    {
        private readonly Histogram<int> _histogram;
        private readonly Counter<int> _meter;
        private Clock _clock;

        public MeterSummary(Counter<int> meter, Histogram<int> histogram)
        {
            _histogram = histogram;
            _meter = meter;
            _clock = new Clock();
        }

        public async Task<int> Record(Func<Task<int>> func)
        {
            var value = await func.Invoke();
            First += 1;
            Second += value;
            _meter.Add(1);
            _histogram.Record(Second);

            return value;
        }
    }

    public class MeterCounter : AbstractMeter
    {
        private readonly Counter<int> _value;

        public MeterCounter(Counter<int> histogram)
        {
            _value = histogram;
        }

        public void Increment()
        {
            var v = 1;
            _value.Add(v);
            First += v;
        }
    }

    public class MeterGauge : AbstractMeter
    {
        private readonly Func<int> _func;
        private ObservableGauge<int> _value;

        public MeterGauge(ObservableGauge<int> histogram, Func<int> func)
        {
            _value = histogram;
            _func = func;
        }

        public override int GetFirst()
        {
            First = _func.Invoke();
            return base.GetFirst();
        }
    }


    public interface IMeterNames
    {
        string AddressableLeaseAbandonTimer { get; }
        string AddressableLeaseRenewalTimer { get; }
        string HealthCheck { get; }
        string PassingHealthChecks { get; }
        string ConnectedClients { get; }
        string PlacementTimer { get; }
        string AddressableCount { get; }
        string NodeCount { get; }
        string ConnectedNodes { get; }
        string MessageSizes { get; }
        string SlowTicks { get; }
        string TickTimer { get; }
        string RetryAttempts { get; }
        string RetryErrors { get; }
    }

    private class MeterNames : IMeterNames
    {
        public string AddressableLeaseAbandonTimer { get; } = "orbit_addressable_lease_abandon";
        public string AddressableLeaseRenewalTimer { get; } = "orbit_addressable_lease_renewal";
        public string HealthCheck { get; } = "orbit_health_checks";
        public string PassingHealthChecks { get; } = "orbit_passing_health_checks";
        public string ConnectedClients { get; } = "orbit_connected_clients";
        public string PlacementTimer { get; } = "orbit_placement_timer";
        public string AddressableCount { get; } = "orbit_addressable_count";
        public string NodeCount { get; } = "orbit_node_count";
        public string ConnectedNodes { get; } = "orbit_connected_nodes";
        public string MessageSizes { get; } = "orbit_message_sizes";
        public string SlowTicks { get; } = "orbit_slow_ticks";
        public string TickTimer { get; } = "orbit_tick_timer";
        public string RetryAttempts { get; } = "orbit_retry_attempts";
        public string RetryErrors { get; } = "orbit_retry_errors";
    }
}