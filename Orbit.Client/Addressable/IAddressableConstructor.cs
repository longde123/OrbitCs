namespace Orbit.Client.Addressable;

public interface IAddressableConstructor
{
    IAddressable ConstructAddressable(Type clazz);
}