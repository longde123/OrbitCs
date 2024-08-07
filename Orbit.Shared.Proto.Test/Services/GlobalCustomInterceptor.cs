using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

public class GlobalCustomInterceptor : Interceptor
{
    private readonly ILogger<GlobalCustomInterceptor> _logger;

    public GlobalCustomInterceptor(ILogger<GlobalCustomInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation(" --- Global Custom Interceptor Invoked  --- ");

        var response = await base.UnaryServerHandler(request, context, continuation);

        _logger.LogInformation(" --- Global Custom Interceptor Completed --- ");

        return response;
    }
}