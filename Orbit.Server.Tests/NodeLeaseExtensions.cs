//
// Copyright (C) 2015 - 2020 Electronic Arts Inc.  All rights reserved.
// This file is part of the Orbit Project <https://www.orbit.cloud>.
// See license in LICENSE.
//

using Orbit.Shared.Mesh;
using Orbit.Util.Time;

namespace Orbit.Server.Tests;

public static class NodeLeaseExtensions
{
    public static NodeLease Expired => new(
        "ChallengeToken.Empty",
        new Timestamp(0L, 0),
        new Timestamp(0L, 0)
    );

    public static NodeLease Forever => new(
        "ChallengeToken.Empty",
        new Timestamp(long.MaxValue, int.MaxValue),
        new Timestamp(long.MaxValue, int.MaxValue)
    );
}