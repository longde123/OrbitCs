using Grpc.Core;
using Grpc.Health.V1;
using Orbit.Util.Concurrent;

namespace Orbit.Server.Service;

public class HealthService : Health.HealthBase
{
    private readonly HealthCheckList _checks;
    private readonly AtomicReference<int> _healthyChecks = new(0);

    public HealthService(HealthCheckList checks)
    {
        this._checks = checks;

        Meters.Gauge(Meters.Names.PassingHealthChecks, () => _healthyChecks.Get());
    }

    public override async Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
    {
        var status = await IsHealthy()
            ? HealthCheckResponse.Types.ServingStatus.Serving
            : HealthCheckResponse.Types.ServingStatus.NotServing;
        return new HealthCheckResponse { Status = status };
    }

    public async Task<bool> IsHealthy()
    {
        var meter = Meters.Timer(Meters.Names.HealthCheck);
        var checks = this._checks.GetChecks();
        var healthyChecksCount = await meter.Record(async () =>
        {
            var checksCount = 0;
            foreach (var check in checks)
            {
                var h = await check.IsHealthy();
                if (h)
                {
                    checksCount++;
                }
            }

            _healthyChecks.AtomicSet(h => checksCount);
            return _healthyChecks.Get();
        });


        return healthyChecksCount == checks.Count();
    }
}