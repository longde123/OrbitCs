// Framework: NUnit
// Technology stack: .NET

using NUnit.Framework;
using Orbit.Util.Time;

namespace Orbit.Util.Misc;

public class ClockTest
{
    [Test]
    public void CheckAdvancingTheClock()
    {
        Task.Run(async () =>
        {
            long advanceTick = 10000;
            var clock = new Clock();
            var start = clock.CurrentTime;
            clock.AdvanceTime(advanceTick);
            var end = clock.CurrentTime;

            Assert.IsTrue(start < end);
        }).GetAwaiter().GetResult();
    }

    [Test]
    public void CheckClockTimePasses()
    {
        Task.Run(async () =>
        {
            long sleepTime = 100;
            var clock = new Clock();
            var start = clock.CurrentTime;

            await Task.Delay((int)sleepTime);

            var end = clock.CurrentTime;
            Assert.IsTrue(start < end);
        }).GetAwaiter().GetResult();
    }
}