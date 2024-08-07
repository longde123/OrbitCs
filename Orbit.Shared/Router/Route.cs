using Orbit.Shared.Mesh;

namespace Orbit.Shared.Router;

public class Route : IEquatable<Route>
{
    public Route(List<NodeId> path)
    {
        Path = path;
    }

    public Route()
    {
        Path = new List<NodeId>();
    }

    public List<NodeId> Path { get; set; }

    public NodeId? NextNode => Path.First();

    public bool Equals(Route? other)
    {
        return other != null && Path.SequenceEqual(other.Path);
    }

    public override bool Equals(object? obj)
    {
        if (obj != null && obj is Route nid)
        {
            return Equals(nid);
        }

        return false;
    }

    public PopResult Pop()
    {
        return new PopResult(new Route(Path.GetRange(1, Path.Count - 1)), Path[Path.Count - 1]);
    }

    public bool IsValid()
    {
        return Path.Count > 0;
    }

    public class PopResult
    {
        public PopResult(Route route, NodeId nodeId)
        {
            Route = route;
            NodeId = nodeId;
        }

        public Route Route { get; set; }
        public NodeId NodeId { get; set; }
    }
}