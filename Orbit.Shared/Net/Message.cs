using Orbit.Shared.Addressable;
using Orbit.Shared.Mesh;
using Orbit.Shared.Router;

namespace Orbit.Shared.Net;

public class Message
{
    public MessageContent Content { get; set; }
    public long? MessageId { get; set; }
    public NodeId? Source { get; set; }
    public MessageTarget Target { get; set; }
    public long Attempts { get; set; }

    public string Destination
    {
        get
        {
            var it = Content switch
            {
                MessageContent.InvocationRequest request => "[InvocationRequest] " + request.Destination.Key,
                MessageContent.InvocationResponse response => "[InvocationResponse] " + Target ?? "",
                _ => ""
            };
            return it;
        }
    }
}

public enum InvocationReason
{
    Invocation = 0,
    Rerouted = 1
}

public static class InvocationReasonExtensions
{
    public static InvocationReason FromInt(int value)
    {
        return (InvocationReason)value;
    }
}

public abstract class MessageTarget
{
    public class BroadUnicast : MessageTarget
    {
        public BroadUnicast( )
        { 
        }
 
    }
    public class Unicast : MessageTarget
    {
        public Unicast(NodeId targetNode)
        {
            TargetNode = targetNode;
        }

        public NodeId TargetNode { get; }
    }

    public class RoutedUnicast : MessageTarget, IEquatable<RoutedUnicast>
    {
        public RoutedUnicast(Route route)
        {
            Route = route;
        }

        public Route Route { get; }

        public bool Equals(RoutedUnicast? obj)
        {
            return Route.Equals(obj.Route);
        }

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is RoutedUnicast nid)
            {
                return Equals(nid);
            }

            return false;
        }
    }
}

public abstract class MessageContent
{
    public class Error : MessageContent
    {
        public Error()
        {
        }

        public Error(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
    
     

    public class ConnectionInfoRequest : MessageContent
    {
    }

    public class ConnectionInfoResponse : MessageContent
    {
        public ConnectionInfoResponse()
        {
        }

        public ConnectionInfoResponse(NodeId nodeId)
        {
            NodeId = nodeId;
        }

        public NodeId NodeId { get; set; }
    }

    public class InvocationRequest : MessageContent, IEquatable<InvocationRequest>
    {
        public InvocationRequest()
        {
        }

        public InvocationRequest(AddressableReference destination, string method, string arguments)
        {
            Destination = destination;
            Method = method;
            Arguments = arguments;
        }

        public InvocationRequest(AddressableReference destination, string method, string arguments,
            InvocationReason reason)
        {
            Destination = destination;
            Method = method;
            Arguments = arguments;
            Reason = reason;
        }

        public AddressableReference Destination { get; set; }
        public string Method { get; set; }
        public string Arguments { get; set; }
        public InvocationReason Reason { get; set; }

        public bool Equals(InvocationRequest? other)
        {
            return Destination.Equals(other.Destination) && Method == other.Method &&
                   Arguments == other.Arguments && Reason == other.Reason;
        }

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is InvocationRequest nid)
            {
                return Equals(nid);
            }

            return false;
        }
    }

    public class InvocationResponse : MessageContent, IEquatable<InvocationResponse>
    {
        public InvocationResponse()
        {
        }

        public InvocationResponse(string data)
        {
            Data = data;
        }

        public string Data { get; set; }

        public bool Equals(InvocationResponse? other)
        {
            return Data == other.Data;
        }

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is InvocationResponse nid)
            {
                return Equals(nid);
            }

            return false;
        }
    }

    public class InvocationResponseError : MessageContent
    {
        public InvocationResponseError()
        {
        }

        public InvocationResponseError(string description, string platform)
        {
            Description = description;
            Platform = platform;
        }

        public string Description { get; set; }
        public string Platform { get; set; }
    }
}