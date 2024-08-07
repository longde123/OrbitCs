using Orbit.Client.Addressable;
using Orbit.Client.Execution;
using Orbit.Client.Mesh;
using Orbit.Util.Di;
using Orbit.Util.Time;

namespace Orbit.Client;

public class OrbitClientConfig
{
    public string GrpcEndpoint { get; set; } = "https://localhost:50056/";
    public string Namespace { get; set; } = "default";
    public Clock Clock { get; set; } = new();
    public TimeSpan TickRate { get; set; } = TimeSpan.FromSeconds(1);
    public int RailCount { get; set; } = 128;
    public int BufferCount { get; set; } = 10_000;
    public List<string> Packages { get; set; } = new();
    public TimeSpan MessageTimeout { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan DeactivationTimeout { get; set; } = TimeSpan.FromSeconds(10);

    public ExternallyConfigured<AddressableDeactivator> AddressableDeactivator { get; set; } =
        new AddressableDeactivator.Instant.Config();

    public TimeSpan AddressableTtl { get; set; } = TimeSpan.FromMinutes(10);

    public ExternallyConfigured<IAddressableConstructor> AddressableConstructor { get; set; } =
        new DefaultAddressableConstructor.DefaultAddressableConstructorSingleton();

    public int NetworkRetryAttempts { get; set; } = 5;
    public TimeSpan JoinClusterTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan LeaveClusterTimeout { get; set; } = TimeSpan.FromSeconds(30);

    public ExternallyConfigured<INodeLeaseRenewalFailedHandler> NodeLeaseRenewalFailedHandler { get; set; } =
        new RestartOnNodeRenewalFailure.RestartOnNodeRenewalFailureSingleton();

    public bool PlatformExceptions { get; set; } = false;


    public Action<ComponentContainerRoot> ContainerOverrides { get; set; } = container => { };
}