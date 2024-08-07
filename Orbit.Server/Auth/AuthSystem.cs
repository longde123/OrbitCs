using Orbit.Server.Mesh;
using Orbit.Shared.Mesh;

namespace Orbit.Server.Auth;

public class AuthInfo
{
    public AuthInfo(bool isManagementNode, NodeId nodeId)
    {
        IsManagementNode = isManagementNode;
        NodeId = nodeId;
    }

    public bool IsManagementNode { get; }
    public NodeId NodeId { get; }
}

public class AuthSystem
{
    public async Task<AuthInfo?> Auth(NodeId? nodeId)
    {
        if (nodeId == null)
        {
            return null;
        }

        var isManagement = nodeId.Namespace == LocalNodeInfo.ManagementNamespace;
        return new AuthInfo(isManagement, nodeId);
    }
}