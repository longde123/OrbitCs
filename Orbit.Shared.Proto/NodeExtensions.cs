using Orbit.Shared.Mesh;

namespace Orbit.Shared.Proto;

public static class NodeExtensions
{
    public static NodeIdProto ToNodeIdProto(this NodeId nodeId)
    {
        return new NodeIdProto
        {
            Key = nodeId.Key,
            Namespace = nodeId.Namespace
        };
    }

    public static NodeId ToNodeId(this NodeIdProto nodeIdProto)
    {
        return new NodeId
        {
            Key = nodeIdProto.Key,
            Namespace = nodeIdProto.Namespace
        };
    }

    public static NodeInfoProto ToNodeInfoProto(this NodeInfo nodeInfo)
    {
        return new NodeInfoProto
        {
            Id = nodeInfo.Id.ToNodeIdProto(),
            VisibleNodes = { nodeInfo.VisibleNodes.Select(node => node.ToNodeIdProto()).ToList() },
            Lease = nodeInfo.Lease.ToNodeLeaseProto(),
            Capabilities = nodeInfo.Capabilities.ToCapabilitiesProto(),
            Status = nodeInfo.NodeStatus.ToNodeStatusProto(),
            Url = nodeInfo.Url
        };
    }

    public static NodeInfo ToNodeInfo(this NodeInfoProto nodeInfoProto)
    {
        return new NodeInfo
        {
            Id = new NodeId(nodeInfoProto.Id.Key, nodeInfoProto.Id.Namespace),
            VisibleNodes = new HashSet<NodeId>(nodeInfoProto.VisibleNodes.Select(node => node.ToNodeId())),
            Lease = nodeInfoProto.Lease.ToLeaseProto(),
            Capabilities = nodeInfoProto.Capabilities.ToCapabilities(),
            Url = nodeInfoProto.Url,
            NodeStatus = nodeInfoProto.Status.ToNodeStatus()
        };
    }

    public static NodeLeaseProto ToNodeLeaseProto(this NodeLease nodeLease)
    {
        return new NodeLeaseProto
        {
            ChallengeToken = nodeLease.ChallengeToken,
            ExpiresAt = nodeLease.ExpiresAt.ToTimestampProto(),
            RenewAt = nodeLease.RenewAt.ToTimestampProto()
        };
    }

    public static NodeLease ToLeaseProto(this NodeLeaseProto nodeLeaseProto)
    {
        return new NodeLease
        {
            ChallengeToken = nodeLeaseProto.ChallengeToken,
            ExpiresAt = nodeLeaseProto.ExpiresAt.ToTimestamp(),
            RenewAt = nodeLeaseProto.RenewAt.ToTimestamp()
        };
    }

    public static CapabilitiesProto ToCapabilitiesProto(this NodeCapabilities nodeCapabilities)
    {
        return new CapabilitiesProto
        {
            AddressableTypes = { nodeCapabilities.AddressableTypes }
        };
    }

    public static NodeCapabilities ToCapabilities(this CapabilitiesProto capabilitiesProto)
    {
        //todo
        //HastSet 2 List
        return new NodeCapabilities
        {
            AddressableTypes = new HashSet<string>(capabilitiesProto.AddressableTypes)
        };
    }

    public static NodeStatusProto ToNodeStatusProto(this NodeStatus nodeStatus)
    {
        return nodeStatus switch
        {
            NodeStatus.Active => NodeStatusProto.Active,
            NodeStatus.Stopped => NodeStatusProto.Stopped,
            NodeStatus.Starting => NodeStatusProto.Starting,
            NodeStatus.Draining => NodeStatusProto.Draining,
            _ => throw new Exception("Unknown node status")
        };
    }

    public static NodeStatus ToNodeStatus(this NodeStatusProto nodeStatusProto)
    {
        return nodeStatusProto switch
        {
            NodeStatusProto.Active => NodeStatus.Active,
            NodeStatusProto.Stopped => NodeStatus.Stopped,
            NodeStatusProto.Starting => NodeStatus.Starting,
            NodeStatusProto.Draining => NodeStatus.Draining,
            _ => throw new Exception("Unknown node status")
        };
    }
}