using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace Orbit.Server.Prometheus;

public class PrometheusMeterEndpoint
{
    public PrometheusMeterEndpoint(string meterName = "HatCo.HatStore", string endpoint = "/metrics", int port = 8080)
    {
        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddMeter(meterName)
            .AddPrometheusHttpListener(options => options.UriPrefixes = new[]
            {
                $"http://localhost:{port}{endpoint}/"
            })
            .Build();

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddSimpleConsole();
        });
        var logger = loggerFactory.CreateLogger<PrometheusMeterEndpoint>();
        logger.LogInformation($"PrometheusMeter http://localhost:{port}{endpoint}/");
    }
}