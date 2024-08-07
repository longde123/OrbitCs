using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

public class SpecificGrpcServiceInterceptor : Interceptor
{
    private readonly ILogger<SpecificGrpcServiceInterceptor> _logger;

    public SpecificGrpcServiceInterceptor(ILogger<SpecificGrpcServiceInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation(" --- Specific GrpcService Interceptor Invoked  --- ");

        var response = await base.UnaryServerHandler(request, context, continuation);

        _logger.LogInformation(" --- Specific GrpcService Interceptor Completed --- ");

        return response;
    }
}