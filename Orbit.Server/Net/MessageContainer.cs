/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using Orbit.Server.Auth;
using Orbit.Shared.Net;

namespace Orbit.Server.Net;

public class MessageMetadata
{
    public MessageMetadata()
    {
    }

    public MessageMetadata(AuthInfo authInfo, MessageDirection messageDirection, bool respondOnError = true)
    {
        AuthInfo = authInfo;
        MessageDirection = messageDirection;
        RespondOnError = respondOnError;
    }

    public AuthInfo AuthInfo { get; set; }
    public MessageDirection MessageDirection { get; set; }
    public bool RespondOnError { get; set; }
}

public class MessageContainer
{
    public MessageContainer(Message message, MessageMetadata metadata)
    {
        Message = message;
        Metadata = metadata;
    }

    public Message Message { get; }
    public MessageMetadata Metadata { get; }

    public override string ToString()
    {
        return $"Message {Message} Metadata {Metadata}  ";
    }
}

public enum MessageDirection
{
    Inbound,
    Outbound
}