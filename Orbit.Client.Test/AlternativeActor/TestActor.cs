using Orbit.Client.Actor;

namespace Orbit.Client.AlternativeActor;

internal interface ITestActor : IActorWithInt32Key
{
    Task<string> Ping(string msg = "");
}

public class TestActorImpl : AbstractActor, ITestActor
{
    public async Task<string> Ping(string msg)
    {
        return await Task.FromResult(msg);
    }
}