using Orbit.Client.Addressable;
using Orbit.Shared.Addressable;

namespace Orbit.Client.Actor;

public class ActorProxyFactory
{
    private readonly AddressableProxyFactory _addressableProxyFactory;

    public ActorProxyFactory(AddressableProxyFactory addressableProxyFactory)
    {
        _addressableProxyFactory = addressableProxyFactory;
    }

    private T CreateProxyInternal<T>(Type type, Key key, bool actorReference) where T : class, IActor
    {
        return _addressableProxyFactory.CreateProxy<T>(type, key, actorReference);
    }

    public T CreateProxy<T>(Type grainType, bool actorReference) where T : class, IActorWithNoKey
    {
        return CreateProxyInternal<T>(grainType, Key.None(), actorReference);
    }

    public T CreateProxy<T>(Type grainType, string key, bool actorReference) where T : class, IActorWithStringKey
    {
        return CreateProxyInternal<T>(grainType, Key.Of(key), actorReference);
    }

    public T CreateProxy<T>(Type grainType, int key, bool actorReference) where T : class, IActorWithInt32Key
    {
        return CreateProxyInternal<T>(grainType, Key.Of(key), actorReference);
    }

    public T CreateProxy<T>(Type grainType, long key, bool actorReference) where T : class, IActorWithInt64Key
    {
        return CreateProxyInternal<T>(grainType, Key.Of(key), actorReference);
    }
}

public static class ActorProxyFactoryExtensions
{
    public static T CreateProxy<T>(this ActorProxyFactory actorProxyFactory) where T : class, IActorWithNoKey
    {
        return actorProxyFactory.CreateProxy<T>(typeof(T), false);
    }

    public static T CreateProxy<T>(this ActorProxyFactory actorProxyFactory, string key)
        where T : class, IActorWithStringKey
    {
        return actorProxyFactory.CreateProxy<T>(typeof(T), key, false);
    }

    public static T CreateProxy<T>(this ActorProxyFactory actorProxyFactory, int key) where T : class, IActorWithInt32Key
    {
        return actorProxyFactory.CreateProxy<T>(typeof(T), key, false);
    }

    public static T CreateProxy<T>(this ActorProxyFactory actorProxyFactory, long key)
        where T : class, IActorWithInt64Key
    {
        return actorProxyFactory.CreateProxy<T>(typeof(T), key, false);
    }


    public static T CreateProxyReference<T>(this ActorProxyFactory actorProxyFactory, string key)
        where T : class, IActorWithStringKey
    {
        return actorProxyFactory.CreateProxy<T>(typeof(T), key, true);
    }
}