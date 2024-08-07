using Orbit.Util.Di;
using Orbit.Util.Time;

namespace Orbit.Client.Execution;

public delegate Task Deactivator(IDeactivatable handle);

public abstract class AddressableDeactivator
{
    public abstract Task Deactivate(List<IDeactivatable> addressables, Deactivator deactivate);

    protected async Task DeactivateItems(
        List<IDeactivatable> addressables,
        int concurrency,
        long deactivationsPerSecond,
        Deactivator deactivate)
    {
        var cts = new CancellationTokenSource();
        var ticker = TickerUtils.HighResolutionTicker<int>(deactivationsPerSecond, cts.Token);
        var tickerAsyncEnumerator = ticker.GetAsyncEnumerator();
        try
        {
            await Task.WhenAll(addressables.Select(async a =>
            {
                await tickerAsyncEnumerator.MoveNextAsync();
                await deactivate(a);
            }));
        }
        finally
        {
            cts.Cancel();
        }
    }

    public class Concurrent : AddressableDeactivator
    {
        private readonly Config _config;

        public Concurrent(Config config)
        {
            this._config = config;
        }

        public override async Task Deactivate(List<IDeactivatable> addressables, Deactivator deactivate)
        {
            DeactivateItems(addressables, _config.ConcurrentDeactivations, long.MaxValue, deactivate);
        }

        public class Config : ExternallyConfigured<AddressableDeactivator>
        {
            public Config(int d)
            {
                ConcurrentDeactivations = d;
            }

            public int ConcurrentDeactivations { get; }
            public override Type InstanceType => typeof(Concurrent);
        }
    }

    public class RateLimited : AddressableDeactivator
    {
        private readonly Config _config;

        public RateLimited(Config config)
        {
            this._config = config;
        }

        public override async Task Deactivate(List<IDeactivatable> addressables, Deactivator deactivate)
        {
            DeactivateItems(addressables, int.MaxValue, _config.DeactivationsPerSecond, deactivate);
        }

        public class Config : ExternallyConfigured<AddressableDeactivator>
        {
            public Config(long deactivationsPerSecond)
            {
                DeactivationsPerSecond = deactivationsPerSecond;
            }

            public long DeactivationsPerSecond { get; }

            public override Type InstanceType => typeof(RateLimited);
        }
    }

    public class TimeSpan : AddressableDeactivator
    {
        private readonly Config _config;

        public TimeSpan(Config config)
        {
            this._config = config;
        }

        public override async Task Deactivate(List<IDeactivatable> addressables, Deactivator deactivate)
        {
            var deactivationsPerSecond = (int)(addressables.Count * 1000 / _config.DeactivationTimeMilliseconds);

            DeactivateItems(addressables, int.MaxValue, deactivationsPerSecond, deactivate);
        }

        public class Config : ExternallyConfigured<AddressableDeactivator>
        {
            public Config(long deactivationTimeMilliseconds)
            {
                DeactivationTimeMilliseconds = deactivationTimeMilliseconds;
            }

            public long DeactivationTimeMilliseconds { get; }

            public override Type InstanceType => typeof(TimeSpan);
        }
    }

    public class Instant : AddressableDeactivator
    {
        public override async Task Deactivate(List<IDeactivatable> addressables, Deactivator deactivate)
        {
            DeactivateItems(addressables, int.MaxValue, long.MaxValue, deactivate);
        }

        public class Config : ExternallyConfigured<AddressableDeactivator>
        {
            public override Type InstanceType => typeof(Instant);
        }
    }
}