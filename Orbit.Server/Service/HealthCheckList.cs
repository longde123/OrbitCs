using Orbit.Server.Mesh;

namespace Orbit.Server.Service;

public class HealthCheckList
{
    private readonly IAddressableDirectory _addressableDirectory;
    private readonly LocalNodeInfo _localNodeInfo;
    private readonly INodeDirectory _nodeDirectory;
    private readonly OrbitServer _server;

    public HealthCheckList(OrbitServer server, LocalNodeInfo localNodeInfo,
        IAddressableDirectory addressableDirectory, INodeDirectory nodeDirectory)
    {
        _server = server;
        _localNodeInfo = localNodeInfo;
        _addressableDirectory = addressableDirectory;
        _nodeDirectory = nodeDirectory;
    }

    public List<IHealthCheck> GetChecks()
    {
        return new List<IHealthCheck>
        {
            _server,
            _localNodeInfo,
            _addressableDirectory,
            _nodeDirectory
        };
    }
}