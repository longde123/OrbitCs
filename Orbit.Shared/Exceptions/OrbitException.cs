using Orbit.Shared.Mesh;
using Orbit.Shared.Net;

namespace Orbit.Shared.OrException;

public class InvalidNodeId : Exception
{
    public InvalidNodeId(NodeId nodeId) : base($"{nodeId} is not valid. Did the lease expire?")
    {
    }
}

public class InvalidChallengeException : Exception
{
    public InvalidChallengeException(NodeId nodeId, string challengeToken) : base($"Invalid challenge for {nodeId}")
    {
    }
}

public class CapacityExceededException : Exception
{
    public CapacityExceededException(string message) : base(message)
    {
    }
}

public class AuthFailed : Exception
{
    public AuthFailed(string message) : base(message)
    {
    }
}

public class PlacementFailedException : Exception
{
    public PlacementFailedException(string message) : base(message)
    {
    }
}

public class RerouteMessageException : Exception
{
    public RerouteMessageException(string message) : base(message)
    {
    }
}

public static class ThrowableExtensions
{
    public static MessageContent.Error ToErrorContent(this Exception ex)
    {
        return new MessageContent.Error(ex?.ToString());
    }
}