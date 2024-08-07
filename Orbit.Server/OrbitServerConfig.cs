using Google.Protobuf.WellKnownTypes;
using Orbit.Server.Mesh;
using Orbit.Server.Mesh.Local;
using Orbit.Server.Service;
using Orbit.Util.Di;
using Orbit.Util.Time;

namespace Orbit.Server;

public class OrbitServerConfig
{
    public bool AcquireShutdownLatch = true;

    public ExternallyConfigured<IAddressableDirectory> AddressableDirectory =
        new LocalAddressableDirectory.LocalAddressableDirectorySingleton();

    public LeaseDuration AddressableLeaseDuration = new(600);
    public Clock Clock = new();

    public int MessageRetryAttempts = 10;
    public ExternallyConfigured<IMeterRegistry> MeterRegistry = new LocalMeterRegistry.LocalMeterRegistrySingleton();


    public ExternallyConfigured<INodeDirectory> NodeDirectory = new LocalNodeDirectory.LocalNodeDirectorySingleton();
    public LeaseDuration NodeLeaseDuration = new(10);
    public int PipelineBufferCount = 10_000;
    public int PipelineRailCount = 32;

    public LocalServerInfo ServerInfo = new(
        Environment.GetEnvironmentVariable("ORBIT_URL") ?? "localhost:50056",
        int.Parse(Environment.GetEnvironmentVariable("ORBIT_PORT") ?? "50056")
    );

    public Duration TickRate = Duration.FromTimeSpan(TimeSpan.FromSeconds(1));

    public Action<ComponentContainerRoot> ContainerOverrides { get; set; } = container => { };
}