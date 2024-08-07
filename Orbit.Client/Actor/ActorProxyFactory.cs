using Orbit.Client.Addressable;
using Orbit.Shared.Addressable;

namespace Orbit.Client.Actor;

public class ActorProxyFactory
{
    private readonly AddressableProxyFactory _addressableProxyFactory;

    public ActorProxyFactory(AddressableProxyFactory addressableProxyFactory)
    {
        this._addressableProxyFactory = addressableProxyFactory;
    }

    private T CreateProxyInternal<T>(Type type, Key key) where T : class, IActor
    {
        return _addressableProxyFactory.CreateProxy<T>(type, key);
    }

    public T CreateProxy<T>(Type grainType) where T : class, IActorWithNoKey
    {
        return CreateProxyInternal<T>(grainType, Key.None());
    }

    public T CreateProxy<T>(Type grainType, string key) where T : class, IActorWithStringKey
    {
        return CreateProxyInternal<T>(grainType, Key.Of(key));
    }

    public T CreateProxy<T>(Type grainType, int key) where T : class, IActorWithInt32Key
    {
        return CreateProxyInternal<T>(grainType, Key.Of(key));
    }

    public T CreateProxy<T>(Type grainType, long key) where T : class, IActorWithInt64Key
    {
        return CreateProxyInternal<T>(grainType, Key.Of(key));
    }
}

public static class ActorProxyFactoryExtensions
{
    public static T CreateProxy<T>(this ActorProxyFactory actorProxyFactory) where T : class, IActorWithNoKey
    {
        return actorProxyFactory.CreateProxy<T>(typeof(T));
    }

    public static T CreateProxy<T>(this ActorProxyFactory actorProxyFactory, string key)
        where T : class, IActorWithStringKey
    {
        return actorProxyFactory.CreateProxy<T>(typeof(T), key);
    }

    public static T CreateProxy<T>(this ActorProxyFactory actorProxyFactory, int key) where T : class, IActorWithInt32Key
    {
        return actorProxyFactory.CreateProxy<T>(typeof(T), key);
    }

    public static T CreateProxy<T>(this ActorProxyFactory actorProxyFactory, long key)
        where T : class, IActorWithInt64Key
    {
        return actorProxyFactory.CreateProxy<T>(typeof(T), key);
    }
}