/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using NUnit.Framework;
using Orbit.Util.Time;

namespace Orbit.Util.Misc;

public class ControlFlowUtilsTest
{
    [Test]
    public void CheckAttemptSuccess()
    {
        var result = AttemptUtil.Attempt(body: () => { return "Some result"; }).GetAwaiter().GetResult();
        Assert.AreEqual(result, "Some result");
    }

    [Test]
    public void CheckAttemptMaxAttempts()
    {
        var attempts = 0;
        Assert.Throws<Exception>(() =>
        {
            AttemptUtil.Attempt<string>(
                5,
                1,
                body: () =>
                {
                    attempts++;
                    throw new Exception("FAIL");
                }
            ).GetAwaiter().GetResult();
        });
        Assert.AreEqual(attempts, 5);
    }

    [Test]
    public void CheckAttemptBackOff()
    {
        var stopwatch = Stopwatch.Start(new Clock());
        Assert.Throws<Exception>(() =>
        {
            AttemptUtil.Attempt<string>(
                5,
                1,
                factor: 2.0,
                body: () => { throw new Exception("FAIL"); }
            ).GetAwaiter().GetResult();
        });

        var elapsed = stopwatch.Elapsed;
        Assert.IsTrue(elapsed > 10);
    }

    [Test]
    public void CheckAttemptMaxDelay()
    {
        var stopwatch = Stopwatch.Start(new Clock());
        Assert.Throws<Exception>(() =>
        {
            AttemptUtil.Attempt<string>(
                5,
                maxDelay: 100,
                initialDelay: 1,
                factor: 1000.0,
                body: () => { throw new Exception("FAIL"); }
            ).GetAwaiter().GetResult();
        });
        var elapsed = stopwatch.Elapsed;
        Assert.IsTrue(elapsed < 1000);
    }

    [Test]
    public void CheckAttemptSuccessAfterFail()
    {
        var attempts = 0;
        var result = AttemptUtil.Attempt<string>(
            5,
            1,
            body: () =>
            {
                if (attempts++ < 3)
                {
                    throw new Exception("FAIL");
                }

                return "Hello";
            }
        ).GetAwaiter().GetResult();

        Assert.AreEqual(result, "Hello");
    }
}