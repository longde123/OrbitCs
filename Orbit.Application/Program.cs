using Orbit.Application.Impl;
using Orbit.Server;
using Orbit.Server.Prometheus;

namespace Orbit.Application;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var settingsLoader = new SettingsLoader();
        var config = settingsLoader.LoadConfig();
        config.MeterRegistry = new PrometheusMetrics.PrometheusMetricsSingleton();
        var server = new OrbitServer(config);

      //  await Task.Delay(TimeSpan.FromSeconds(5));
        await server.Start();
    }
}