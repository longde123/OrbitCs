/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using Orbit.Shared.Net;

namespace Orbit.Server.Pipeline.Step;

public class EchoStepIn : PipelineStepIn
{
    public override async Task<bool> OnInbound(PipelineContext context, Message msg)
    {
        if (msg.Source != null)
        {
            var source = msg.Source;

            msg.Target = new MessageTarget.Unicast(source);


            return false;
        }

        return true;
    }
}