using Orbit.Shared.Net;
using Orbit.Shared.Router;

namespace Orbit.Shared.Proto;

public static class MessageExtensions
{
    public static Message ToMessage(this MessageProto messageProto)
    {
        return new Message
        {
            MessageId = messageProto.MessageId,
            Source = messageProto.Source?.ToNodeId(),
            Target = messageProto.Target?.ToMessageTarget(),
            Content = messageProto.Content.ToMessageContent(),
            Attempts = messageProto.Attempts
        };
    }

    public static MessageProto ToMessageProto(this Message message)
    {
        return new MessageProto
        {
            MessageId = message.MessageId ?? 0,
            Source = message.Source?.ToNodeIdProto(),
            Target = message.Target?.ToMessageTargetProto(),
            Attempts = message.Attempts,
            Content = message.Content.ToMessageContentProto()
        };
    }

    public static MessageTarget? ToMessageTarget(this MessageTargetProto messageTargetProto)
    {
        switch ((int)messageTargetProto.TargetCase)
        {
            case MessageTargetProto.UnicastTargetFieldNumber:
                return new MessageTarget.Unicast(messageTargetProto.UnicastTarget.Target.ToNodeId());
            case MessageTargetProto.RoutedUnicastTargetFieldNumber:
                return new MessageTarget.RoutedUnicast(new Route(messageTargetProto.RoutedUnicastTarget.Target
                    .Select(node => node.ToNodeId()).ToList()));
            default:
                return null;
        }
    }

    public static MessageTargetProto ToMessageTargetProto(this MessageTarget messageTarget)
    {
        var it = new MessageTargetProto();

        switch (messageTarget)
        {
            case MessageTarget.Unicast unicast:
                it.UnicastTarget = new MessageTargetProto.Types.Unicast
                {
                    Target = unicast.TargetNode.ToNodeIdProto()
                };
                break;
            case MessageTarget.RoutedUnicast routedUnicast:
                it.RoutedUnicastTarget = new MessageTargetProto.Types.RoutedUnicast();
                it.RoutedUnicastTarget.Target.AddRange(
                    routedUnicast.Route.Path.Select(node => node.ToNodeIdProto()));
                break;
            default:
                throw new InvalidOperationException("Unknown message target type");
        }

        return it;
    }

    public static MessageContent ToMessageContent(this MessageContentProto messageContentProto)
    {
        if (messageContentProto.InvocationRequest != null)
        {
            return new MessageContent.InvocationRequest
            {
                Method = messageContentProto.InvocationRequest.Method,
                Arguments = messageContentProto.InvocationRequest.Arguments,
                Destination = messageContentProto.InvocationRequest.Reference.ToAddressableReference(),
                Reason = (InvocationReason)messageContentProto.InvocationRequest.Reason
            };
        }

        if (messageContentProto.InvocationResponse != null)
        {
            return new MessageContent.InvocationResponse
            {
                Data = messageContentProto.InvocationResponse.Value
            };
        }

        if (messageContentProto.Error != null)
        {
            return new MessageContent.Error
            {
                Description = messageContentProto.Error.Description
            };
        }

        if (messageContentProto.InvocationResponseError != null)
        {
            return new MessageContent.InvocationResponseError
            {
                Description = messageContentProto.InvocationResponseError.Description,
                Platform = messageContentProto.InvocationResponseError.Platform
            };
        }

        if (messageContentProto.InfoRequest != null)
        {
            return new MessageContent.ConnectionInfoRequest();
        }

        if (messageContentProto.InfoResponse != null)
        {
            return new MessageContent.ConnectionInfoResponse
            {
                NodeId = messageContentProto.InfoResponse.NodeId.ToNodeId()
            };
        }

        throw new Exception("Unknown message type");
    }

    public static MessageContentProto ToMessageContentProto(this MessageContent messageContent)
    {
        var builder = new MessageContentProto();

        switch (messageContent)
        {
            case MessageContent.InvocationRequest invocationRequest:
                builder.InvocationRequest = new InvocationRequestProto
                {
                    Reference = invocationRequest.Destination.ToAddressableReferenceProto(),
                    Method = invocationRequest.Method,
                    Arguments = invocationRequest.Arguments,
                    Reason = (InvocationReasonProto)(int)invocationRequest.Reason
                };
                break;

            case MessageContent.InvocationResponse invocationResponse:
                builder.InvocationResponse = new InvocationResponseProto
                {
                    Value = invocationResponse.Data
                };

                break;

            case MessageContent.InvocationResponseError invocationResponseError:

                builder.InvocationResponseError = new InvocationResponseErrorProto
                {
                    Description = invocationResponseError.Description,
                    Platform = invocationResponseError.Platform
                };
                break;

            case MessageContent.Error error:
                builder.Error = new ErrorProto
                {
                    Description = error.Description
                };

                break;

            case MessageContent.ConnectionInfoRequest connectionInfoRequest:
                builder.InfoRequest = new ConnectionInfoRequestProto();

                break;

            case MessageContent.ConnectionInfoResponse connectionInfoResponse:
                builder.InfoResponse = new ConnectionInfoResponseProto
                {
                    NodeId = connectionInfoResponse.NodeId.ToNodeIdProto()
                };
                break;
        }

        return builder;
    }
}