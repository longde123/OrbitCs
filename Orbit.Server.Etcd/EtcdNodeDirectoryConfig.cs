using Orbit.Server.Mesh;
using Orbit.Util.Di;

namespace Orbit.Server.Etcd;

public class EtcdNodeDirectoryConfig : ExternallyConfigured<INodeDirectory>
{
    public string Url { get; set; }

    public (TimeSpan, TimeSpan) CleanupFrequencyRange { get; set; } =
        (TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2));

    public override Type InstanceType => typeof(EtcdNodeDirectory);
}