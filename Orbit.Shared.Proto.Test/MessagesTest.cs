// This code snippet is a translation from Kotlin to C#.

/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using NUnit.Framework;
using Orbit.Shared.Addressable;
using Orbit.Shared.Net;

namespace Orbit.Shared.Proto;

public class MessagesTest
{
    [Test]
    public void TestInvocationRequestMessageContentConversion()
    {
        var initialRef = new MessageContent.InvocationRequest(
            method: "test",
            arguments: "[]",
            destination: new AddressableReference("refType", new Key.StringKey("refId")),
            reason: InvocationReason.Rerouted
        );
        var convertedRef = initialRef.ToMessageContentProto();
        var endRef = convertedRef.ToMessageContent();
        Assert.AreEqual(initialRef, endRef);
    }

    [Test]
    public void TestInvocationResponseMessageContentConversion()
    {
        var initialRef = new MessageContent.InvocationResponse("test");
        var convertedRef = initialRef.ToMessageContentProto();
        var endRef = convertedRef.ToMessageContent();
        Assert.AreEqual(initialRef, endRef);
    }
}