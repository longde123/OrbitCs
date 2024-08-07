/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using Google.Protobuf.WellKnownTypes;

namespace Orbit.Shared.Proto;

using TimestampProto = Timestamp;

public static class TimestampExtensions
{
    public static Util.Time.Timestamp ToTimestamp(this TimestampProto timestamp)
    {
        return new Util.Time.Timestamp
        {
            Seconds = timestamp.Seconds,
            Nanos = timestamp.Nanos
        };
    }

    public static TimestampProto ToTimestampProto(this Util.Time.Timestamp timestamp)
    {
        return new TimestampProto
        {
            Seconds = timestamp.Seconds,
            Nanos = timestamp.Nanos
        };
    }
}