/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using Orbit.Util.Misc;

namespace Orbit.Shared.Mesh;

public class NodeId : IEquatable<NodeId>
{
    public NodeId()
    {
    }

    public NodeId(string key, string @namespace)
    {
        Key = key;
        Namespace = @namespace;
    }

    public string Key { get; set; }
    public string Namespace { get; set; }

    public bool Equals(NodeId? other)
    {
        return other != null && Key == other.Key && Namespace == other.Namespace;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            // From http://stackoverflow.com/a/263416/613130
            var hash = 17;
            hash = hash * 23 + Namespace.GetHashCode();
            hash = hash * 23 + Key.GetHashCode();
            return hash;
        }
    }

    public static NodeId Generate(string @namespace)
    {
        return new NodeId(RngUtils.RandomString(), @namespace);
    }

    public override bool Equals(object? obj)
    {
        if (obj != null && obj is NodeId nid)
        {
            return Equals(nid);
        }

        return false;
    }

    public override string ToString()
    {
        return $"({Namespace}:{Key})";
    }
}