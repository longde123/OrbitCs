using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orbit.Server;

namespace Orbit.Application.Impl;

internal class SettingsLoader
{
    private static readonly ILogger Logger = new LoggerFactory().CreateLogger<SettingsLoader>();


    public OrbitServerConfig LoadConfig()
    {
        Logger.LogInformation("Searching for Orbit Settings...");
        var settingsFileEnv = Environment.GetEnvironmentVariable("ORBIT_SETTINGS");
        if (!string.IsNullOrWhiteSpace(settingsFileEnv))
        {
            var path = Path.GetFullPath(settingsFileEnv);
            if (File.Exists(path))
            {
                var fileContent = File.ReadAllText(path);
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                var configuration = JsonConvert.DeserializeObject<OrbitServerConfig>(fileContent, settings);
                return configuration;
            }
        }

        Logger.LogInformation("No settings found. Using defaults.");
        return new OrbitServerConfig();
    }
}