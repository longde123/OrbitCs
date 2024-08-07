// C# code translated from Kotlin

/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using NUnit.Framework;

namespace Orbit.Util.Misc;

public class RngUtilsTest
{
    [Test]
    public void CheckRandomStringGeneratesUniqueIds()
    {
        var firstString = RngUtils.RandomString();
        var secondString = RngUtils.RandomString();
        Assert.IsTrue(firstString.Length > 0);
        Assert.IsTrue(secondString.Length > 0);
        Assert.AreNotEqual(firstString, secondString);
    }

    [Test]
    public void CheckRandomStringGeneratesUniqueIds1000Times()
    {
        var ids = new List<string>();
        for (var i = 0; i < 1000; i++)
        {
            ids.Add(RngUtils.RandomString());
        }

        Assert.AreEqual(ids.Count, new HashSet<string>(ids).Count);
    }
}