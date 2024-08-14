using NUnit.Framework;
using Orbit.Client.Actor;
using Orbit.Client.Reactive;

namespace Orbit.Client.Test;

public class ActorSubjectTest : BaseIntegrationTest
{
    [Test]
    public async Task TestBasicActorSubjectRequestResponse()
    {
        var actor = Client.ActorFactory.CreateProxyReference<ITestActorSubject>("test");
        var observer = new AsyncSubscribe<string>((s) => { Console.WriteLine(s); }, (err) => { Console.WriteLine(err); });
        await actor.Subscribe(observer);
        await actor.OnNext("test");
        await actor.OnError(new Exception("test"));
        await actor.UnSubscribe(observer);
        Assert.AreEqual("Hello Joe", "Hello Joe");
    }
}