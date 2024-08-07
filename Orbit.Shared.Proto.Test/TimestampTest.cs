// Framework: NUnit

/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using NUnit.Framework;
using Orbit.Util.Time;

namespace Orbit.Shared.Proto;

[TestFixture]
public class TimestampTest
{
    [Test]
    public void TestTimestampConversion()
    {
        var initialRef = Timestamp.Now();
        var convertedRef = initialRef.ToTimestampProto();
        var endRef = convertedRef.ToTimestamp();
        Assert.AreEqual(initialRef, endRef);
    }
}