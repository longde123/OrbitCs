/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using Orbit.Server.Service;
using Orbit.Shared.Mesh;
using Orbit.Util.Concurrent;

namespace Orbit.Server.Mesh;

public interface INodeDirectory : IAsyncMap<NodeId, NodeInfo>, IHealthCheck
{
    Task<IEnumerable<(NodeId, NodeInfo)>> Entries();
    Task Tick();
}