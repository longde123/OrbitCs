/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using Orbit.Shared.Mesh;
using Orbit.Util.Concurrent;

namespace Orbit.Client.Net;

public class LocalNode
{
    private readonly OrbitClientConfig _config;
    private readonly NodeData _defaultNodeData;

    private readonly AtomicReference<NodeData> _refNodeData;

    public LocalNode(OrbitClientConfig config)
    {
        _config = config;
        _defaultNodeData = new NodeData(config.GrpcEndpoint, config.Namespace);
        _refNodeData = new AtomicReference<NodeData>(_defaultNodeData);
    }

    public NodeData Status => _refNodeData.Get();

    public NodeData Manipulate(Func<NodeData, NodeData> body)
    {
        return _refNodeData.AtomicSet(body);
    }

    public void Reset()
    {
        Manipulate(_ => _defaultNodeData);
    }
}

public class NodeData
{
    public NodeCapabilities? Capabilities = null;
    public ClientState ClientState = ClientState.Idle;
    public string GrpcEndpoint;

    public string Namespace;
    public NodeInfo? NodeInfo = null;

    public NodeData()
    {
    }

    public NodeData(string grpcEndpoint, string ns)
    {
        GrpcEndpoint = grpcEndpoint;
        Namespace = ns;
    }
}