using Orbit.Shared.Mesh;
using Orbit.Util.Time;

namespace Orbit.Shared.Addressable;

public class AddressableLease
{
    public AddressableLease()
    {
    }

    public AddressableLease(NodeId nodeId, AddressableReference reference, Timestamp expiresAt, Timestamp renewAt)
    {
        NodeId = nodeId;
        Reference = reference;
        ExpiresAt = expiresAt;
        RenewAt = renewAt;
    }

    public NodeId NodeId { get; set; }
    public AddressableReference Reference { get; set; }
    public Timestamp ExpiresAt { get; set; }
    public Timestamp RenewAt { get; set; }
}