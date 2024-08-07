using System.Reflection;

namespace Orbit.Client.Addressable;

using AddressableClass = Type;

public class AddressableInterfaceDefinition
{
    public AddressableClass InterfaceClass { get; set; }
    public Dictionary<MethodInfo, AddressableInterfaceMethodDefinition> Methods { get; set; }
}

public class AddressableInterfaceMethodDefinition
{
    public MethodInfo Method { get; set; }
}

public class AddressableImplDefinition
{
    public AddressableClass ImplClass { get; set; }
    public AddressableClass InterfaceClass { get; set; }
    public AddressableInterfaceDefinition InterfaceDefinition { get; set; }
    public Dictionary<MethodInfo, AddressableImplMethodDefinition> Methods { get; set; }
    public AddressableImplMethodDefinition OnActivateMethod { get; set; }
    public AddressableImplMethodDefinition OnDeactivateMethod { get; set; }
}

public class AddressableImplMethodDefinition
{
    public MethodInfo Method { get; set; }
    public bool IsOnActivate { get; set; }
    public bool IsOnDeactivate { get; set; }
}