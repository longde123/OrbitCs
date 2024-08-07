/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using Orbit.Shared.Net;
using Orbit.Shared.Router;

namespace Orbit.Server.Net;

public interface IMessageSender
{
    Task SendMessage(Message message, Route? route = null);
}