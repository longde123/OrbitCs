/*
 Copyright (C) 2015 - 2020 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using NUnit.Framework;

namespace Orbit.Util.Misc;

[TestFixture]
public class RetryTest
{
    [Test]
    public void WillAttemptMultipleTimes()
    {
        Task.Run(async () =>
        {
            var count = 0;
            await RetryUtil.Retry(1, 5, async () =>
            {
                if (++count < 5)
                {
                    throw new Exception("Fail");
                }

                return 0;
            });

            Assert.AreEqual(5, count);
        }).GetAwaiter().GetResult();
    }

    [Test]
    public void WillStopAttemptingAfterSpecifiedAmount()
    {
        var count = 0;
        Assert.Throws<RetriesExceededException>(() =>
        {
            Task.Run(async () =>
            {
                await RetryUtil.Retry<dynamic>(1, 5, async () =>
                {
                    count++;
                    throw new Exception("Fail");
                });
            }).GetAwaiter().GetResult();
        });
        Assert.AreEqual(5, count);
    }
}