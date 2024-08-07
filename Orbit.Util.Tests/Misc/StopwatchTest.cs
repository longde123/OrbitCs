// C# code translated from Kotlin

/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using NUnit.Framework;
using Orbit.Util.Time;

namespace Orbit.Util.Misc;

public class StopwatchTest
{
    [Test]
    public async Task CheckStopwatchTimePasses()
    {
        long sleepTime = 100;
        var clock = new Clock();
        var stopwatch = Stopwatch.Start(clock);
        await Task.Delay(TimeSpan.FromMilliseconds(sleepTime));
        long elapsed = stopwatch.Elapsed;
        Assert.IsTrue(elapsed >= sleepTime);
    }
}