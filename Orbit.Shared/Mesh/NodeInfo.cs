namespace Orbit.Shared.Mesh;

public class NodeInfo : IEquatable<NodeInfo>
{
    public NodeInfo()
    {
        VisibleNodes = new HashSet<NodeId>();
    }

    public NodeInfo(NodeId id, NodeCapabilities capabilities, string? url = null, NodeLease? lease = null,
        HashSet<NodeId>? visibleNodes = null, NodeStatus nodeStatus = NodeStatus.Stopped)
    {
        Id = id;
        Capabilities = capabilities;
        Url = url;
        Lease = lease;
        VisibleNodes = visibleNodes ?? new HashSet<NodeId>();
        NodeStatus = nodeStatus;
    }

    public NodeId Id { get; set; }
    public NodeCapabilities Capabilities { get; set; }
    public string? Url { get; set; }
    public NodeLease Lease { get; set; }
    public HashSet<NodeId> VisibleNodes { get; set; }
    public NodeStatus NodeStatus { get; set; }

    public bool Equals(NodeInfo? other)
    {
        //todo
        return other != null
               && Lease != null
               && Id.Equals(other.Id)
               && VisibleNodes.SetEquals(other.VisibleNodes)
               && Lease.Equals(other.Lease);
    }

    public override bool Equals(object? obj)
    {
        if (obj != null && obj is NodeInfo nid)
        {
            return Equals(nid);
        }

        return false;
    }
}