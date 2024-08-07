/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using Orbit.Shared.Mesh;
using Orbit.Shared.Proto;

public static class NodeInfoExtensions
{
    public static NodeLeaseResponseProto ToNodeLeaseRequestResponseProto(this NodeInfo nodeInfo)
    {
        var nodeLeaseResponseProto = new NodeLeaseResponseProto
        {
            Status = NodeLeaseResponseProto.Types.Status.Ok,
            Info = nodeInfo.ToNodeInfoProto()
        };
        return nodeLeaseResponseProto;
    }

    public static NodeLeaseResponseProto ToNodeLeaseRequestResponseProto(this Exception throwable)
    {
        return new NodeLeaseResponseProto
        {
            Status = NodeLeaseResponseProto.Types.Status.Error,
            ErrorDescription = throwable.ToString()
        };
    }
}