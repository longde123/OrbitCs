namespace Orbit.Shared.Mesh;

using AddressableType = String;

public class NodeCapabilities
{
    public NodeCapabilities()
    {
    }

    public NodeCapabilities(HashSet<AddressableType> addressableTypes)
    {
        AddressableTypes = addressableTypes;
    }

    public HashSet<AddressableType> AddressableTypes { get; set; }
}