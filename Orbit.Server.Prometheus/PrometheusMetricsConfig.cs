using Orbit.Util.Di;

namespace Orbit.Server.Prometheus;

public class PrometheusMetricsConfig : ExternallyConfigured<PrometheusMetrics>
{
    public string Url { get; set; } = "/metrics";
    public int Port { get; set; } = 9090;
    public string Name { get; set; } = "HatCo.HatStore";
    public override Type InstanceType => typeof(PrometheusMetrics);
}