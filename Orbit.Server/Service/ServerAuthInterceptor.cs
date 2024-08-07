using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Orbit.Shared.Mesh;
using Orbit.Shared.Proto;

namespace Orbit.Server.Service;

public class ServerAuthInterceptor : Interceptor
{
    public static readonly string NodeKey = Headers.NodeKeyName;
    public static readonly string Namespace = Headers.NamespaceName;
    public static readonly string NodeId = "nodeId";
    private readonly ILogger<ServerAuthInterceptor> _logger;

    public ServerAuthInterceptor(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ServerAuthInterceptor>();
    }

    private void UserStateHandler(ref ServerCallContext context)
    {
        var nodeKey = context.RequestHeaders.GetValue(Headers.NodeKeyName);
        var nameSpace = context.RequestHeaders.GetValue(Headers.NamespaceName);


        context.UserState.Add(Namespace, nameSpace);
        context.UserState.Add(NodeKey, nodeKey);

        if (!string.IsNullOrEmpty(nameSpace) && !string.IsNullOrEmpty(nodeKey))
        {
            context.UserState.Add(NodeId, new NodeId(nodeKey, nameSpace));
        }
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        LogCall<TRequest, TResponse>(MethodType.Unary, context);
        UserStateHandler(ref context);
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            // Note: The gRPC framework also logs exceptions thrown by handlers to .NET Core logging.
            _logger.LogError(ex, $"Error thrown by {context.Method}.");

            throw;
        }
    }

    public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        LogCall<TRequest, TResponse>(MethodType.ClientStreaming, context);
        UserStateHandler(ref context);
        return base.ClientStreamingServerHandler(requestStream, context, continuation);
    }

    public override Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        LogCall<TRequest, TResponse>(MethodType.ServerStreaming, context);
        UserStateHandler(ref context);
        return base.ServerStreamingServerHandler(request, responseStream, context, continuation);
    }

    public override Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        LogCall<TRequest, TResponse>(MethodType.DuplexStreaming, context);
        UserStateHandler(ref context);
        return base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);
    }

    private void LogCall<TRequest, TResponse>(MethodType methodType, ServerCallContext context)
        where TRequest : class
        where TResponse : class
    {
        _logger.LogWarning(
            $"Starting call. Type: {methodType}. Request: {typeof(TRequest)}. Response: {typeof(TResponse)}");
    }
}