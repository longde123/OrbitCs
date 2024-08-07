using Orbit.Server.Mesh;
using Orbit.Shared.Mesh;
using Orbit.Shared.Router;

namespace Orbit.Server.Router;

public class Router
{
    private readonly ClusterManager _clusterManager;
    private readonly LocalNodeInfo _localNode;

    public Router()
    {
    }

    public Router(LocalNodeInfo localNode, ClusterManager clusterManager)
    {
        this._localNode = localNode;
        this._clusterManager = clusterManager;
    }

    public virtual Route FindRoute(NodeId targetNode, Route possibleRoute = null)
    {
        var path = _clusterManager.FindRoute(_localNode.Info.Id, targetNode);

        return new Route(path);
    }
}