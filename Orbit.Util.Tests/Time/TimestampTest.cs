// Technology stack: C#

/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using NUnit.Framework;
using Orbit.Util.Time;

namespace Orbit.Util.Tests.Time;

public class TimestampTest
{
    [Test]
    public void TestTimestampComparison()
    {
        var first = Timestamp.Now();
        Thread.Sleep(100);
        var later = Timestamp.Now();

        Assert.AreEqual(first, first);
        Assert.IsTrue(first < later);
        Assert.IsTrue(later > first);
    }

    [Test]
    public void TestTimestampConversionToInstant()
    {
        var initial = Timestamp.Now();
        var converted = initial.ToDateTime();
        var final = converted.ToTimestamp();
        Assert.AreEqual(initial, final);
    }

    [Test]
    public void TestInstantConversionToTimestamp()
    {
        var initial = Timestamp.Now().ToDateTime();
        ;
        var converted = initial.ToTimestamp();
        var final = converted.ToDateTime();
        Assert.AreEqual(initial, final);
    }
}