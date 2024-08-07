using Orbit.Server.Service;
using Orbit.Util.Di;

namespace Orbit.Server.Prometheus;

public class PrometheusMetrics : IMeterRegistry
{
    public PrometheusMetrics(PrometheusMetricsConfig config)
    {
        Config = config;
    }

    private PrometheusMeterEndpoint MeterServer { get; set; }
    private PrometheusMetricsConfig Config { get; }

    public string GetName()
    {
        return Config.Name;
    }


    public void Init()
    {
        MeterServer = new PrometheusMeterEndpoint(Config.Name, Config.Url, Config.Port);
    }


    public class PrometheusMetricsSingleton : ExternallyConfigured<IMeterRegistry>
    {
        public override Type InstanceType => typeof(PrometheusMetrics);
    }
}