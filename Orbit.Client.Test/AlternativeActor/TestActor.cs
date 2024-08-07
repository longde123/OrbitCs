using Orbit.Client.Actor;

namespace Orbit.Client.AlternativeActor;

internal interface ITestActor : IActorWithStringKey
{
}

public class TestActorImpl : AbstractActor, ITestActor
{
}