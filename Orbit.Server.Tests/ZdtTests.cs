/*
 Copyright (C) 2015 - 2020 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using NUnit.Framework;
using Orbit.Shared.Addressable;

namespace Orbit.Server;

public class ZdtTests : BaseServerTest
{
    [Test]
    public async Task WhenClientLeavesClusterMessagesStillRoutedToClient()
    {
        var receivedMessages = 0.0;

        await StartServer();
        var client1 = await StartClient(message => receivedMessages++);
        await client1.SendMessage("test message 1", "address 1");

        await Task.Delay(TimeSpan.FromSeconds(1));
        Assert.AreEqual(receivedMessages, 1.0);

        var client2 = await StartClient(message => { throw new Exception("Client 2 should not receive a message"); });

        await client1.Drain();
        await client2.SendMessage("test message 2", "address 1");
        await Task.Delay(TimeSpan.FromSeconds(1));
        Assert.AreEqual(receivedMessages, 2.0);
    }

    [Test]
    public async Task WhenClientLeavesClusterAddressablesNotPlacedOnNode()
    {
        var receivedMessagesClient1 = 0.0;
        var receivedMessagesClient2 = 0.0;

        await StartServer();
        var client1 = await StartClient(message => receivedMessagesClient1++);
        await client1.SendMessage("test message 1", "address 1");
        await Task.Delay(TimeSpan.FromSeconds(5));
        Assert.AreEqual(receivedMessagesClient1, 1.0);

        var client2 = await StartClient(message => receivedMessagesClient2++);

        await client1.Drain();
        AdvanceTime(TimeSpan.FromSeconds(5));

        await client2.SendMessage("test message 2", "address 1");
        await Task.Delay(TimeSpan.FromSeconds(5));
        Assert.AreEqual(receivedMessagesClient2, 1.0);
    }

    [Test]
    public async Task WhenClientLeavesClusterCanContinueRenewingAddressableLease()
    {
        var receivedMessages = 0.0;

        await StartServer();
        var client1 = await StartClient(message => receivedMessages++);
        await client1.SendMessage("test message 1", "address 1");
        await Task.Delay(TimeSpan.FromSeconds(1));
        Assert.AreEqual(receivedMessages, 1.0);

        await client1.Drain();

        var lease = await client1.RenewAddressableLease("address 1");

        Assert.AreEqual(lease?.Reference?.Key, Key.Of("address 1"));
    }

    [Test]
    public async Task WhenClientLeavesClusterCanContinueRenewingNodeLease()
    {
        var receivedMessages = 0.0;

        await StartServer();
        var client1 = await StartClient(message => receivedMessages++);
        await client1.SendMessage("test message 1", "address 1");
        await Task.Delay(TimeSpan.FromSeconds(3));
        Assert.AreEqual(receivedMessages, 1.0);

        await client1.Drain();

        var nodeInfo = await client1.RenewNodeLease();

        Assert.AreEqual(nodeInfo?.Id, client1.NodeId);
    }
}