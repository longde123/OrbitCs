using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Orbit.Shared.Proto;

public class GrpcTest
{
    [Test]
    public async Task TestServerClientGreeter()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Trace); // 设置最低日志级别
        });

        var greeterService = new GreeterService(loggerFactory, new GreeterServiceTest());
        var builder = WebApplication.CreateBuilder();
        builder.Services.Add(new ServiceDescriptor(typeof(GreeterService), greeterService));
        builder.Services
            .AddGrpc(c => c.Interceptors.Add<GlobalCustomInterceptor>())
            .AddServiceOptions<GreeterService>(c => c.Interceptors.Add<SpecificGrpcServiceInterceptor>());
        builder.WebHost.UseUrls("https://localhost:5001");
        var app = builder.Build();
        app.UseMiddleware<CustomMiddleware>();
        app.MapGrpcService<GreeterService>();

        Task.Run(() => app.RunAsync());


        var httpHandler = new HttpClientHandler();
        httpHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        using var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions
        {
            HttpClient = null,
            HttpHandler = httpHandler
        });
        var client = new Greeter.GreeterClient(channel);

        var reply = await client.SayHelloAsync(new HelloRequest
        {
            Name = "GreeterClient"
        });
        Console.WriteLine("Greeting: " + reply.Message);

        Console.WriteLine("Shutting down");
        Console.WriteLine("Press any key to exit...");

        Assert.AreEqual(reply.Message, "Hello " + "GreeterClient");
    }
}