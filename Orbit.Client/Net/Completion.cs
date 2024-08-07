/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

namespace Orbit.Client.Net;

public class Completion : TaskCompletionSource<dynamic>
{
    public Completion(TaskCreationOptions creationOptions = TaskCreationOptions.RunContinuationsAsynchronously) : base(
        creationOptions)
    {
    }
}