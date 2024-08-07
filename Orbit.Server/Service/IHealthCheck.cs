namespace Orbit.Server.Service;

public interface IHealthCheck
{
    Task<bool> IsHealthy();
}