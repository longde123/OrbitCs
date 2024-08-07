using Orbit.Util.Di;

namespace Orbit.Client.Addressable;

public class DefaultAddressableConstructor : IAddressableConstructor
{
    public IAddressable ConstructAddressable(Type clazz)
    {
        return (IAddressable)Activator.CreateInstance(clazz);
    }

    public class DefaultAddressableConstructorSingleton : ExternallyConfigured<IAddressableConstructor>
    {
        public override Type InstanceType => typeof(DefaultAddressableConstructor);
    }
}