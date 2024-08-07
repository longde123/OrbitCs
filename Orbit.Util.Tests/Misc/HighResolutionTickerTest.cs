// C# code translated from Kotlin

/*
 Copyright (C) 2015 - 2020 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using NUnit.Framework;
using Orbit.Util.Time;

namespace Orbit.Util.Misc;

public class HighResolutionTickerTest
{
    [Test]
    public void ShouldTickExpectedNumberOfTimes()
    {
        var stopwatch = Stopwatch.Start(new Clock());

        var runBlocking = Task.Run(async () =>
        {
            var cts = new CancellationTokenSource();
            var ticker = TickerUtils.HighResolutionTicker<long>(10000, cts.Token);
            var e = ticker.GetAsyncEnumerator();
            var i = 1;

            try
            {
                while (i++ < 10000)
                {
                    await e.MoveNextAsync();
                }
            }
            finally
            {
                cts.Cancel();
            }
        });
        runBlocking.GetAwaiter().GetResult();

        var elapsed = stopwatch.Elapsed;
        Console.WriteLine($"Elapsed {elapsed}");
        Assert.Less(elapsed, 1050);
        Assert.Greater(elapsed, 950);
    }
}