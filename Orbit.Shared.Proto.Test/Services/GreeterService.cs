using Grpc.Core;
using Microsoft.Extensions.Logging;
using Orbit.Shared.Proto;

public class GreeterServiceTest
{
}

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger _logger;

    public GreeterService(ILoggerFactory loggerFactory, GreeterServiceTest t)
    {
        _logger = loggerFactory.CreateLogger<GreeterService>();
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Sending hello to {request.Name}");
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}