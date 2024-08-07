using Microsoft.Extensions.Logging;
using Orbit.Server.Service;
using Orbit.Util.Di;

namespace Orbit.Server.Mesh.Local;

// Translated from Kotlin to C# using .NET framework

public class LocalMeterRegistry : IMeterRegistry
{
    private static readonly LocalMeterRegistrySingleton Instance = new();
    private readonly ILogger<LocalMeterRegistry> _logger;

    public LocalMeterRegistry(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<LocalMeterRegistry>();
    }

    public string GetName()
    {
        return "HatCo.HatStore";
    }

    public void Init()
    {
        _logger.LogInformation("Starting simple meter registry");
    }

    public class LocalMeterRegistrySingleton : ExternallyConfigured<IMeterRegistry>
    {
        public override Type InstanceType => typeof(LocalMeterRegistry);
    }
}