using Orbit.Client.Addressable;

namespace Orbit.Client.Actor;

[NonConcrete]
public interface IActor : Addressable.IAddressable
{
}

[NonConcrete]
public interface IActorWithNoKey : IActor
{
}

[NonConcrete]
public interface IActorWithStringKey : IActor
{
}

[NonConcrete]
public interface IActorWithInt32Key : IActor
{
}

[NonConcrete]
public interface IActorWithInt64Key : IActor
{
}

public abstract class AbstractActor : AbstractAddressable
{
}