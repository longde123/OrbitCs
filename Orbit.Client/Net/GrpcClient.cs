using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;

namespace Orbit.Client.Net;

public class GrpcClient
{
    public GrpcClient(LocalNode localNode, ClientAuthInterceptor authInterceptor, OrbitClientConfig config)
    {
        var httpHandler = new HttpClientHandler();
        httpHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        var options = new GrpcChannelOptions
        {
            MaxRetryAttempts = config.NetworkRetryAttempts,
            HttpClient = null,
            HttpHandler = httpHandler
        };
        GrpcChannel = GrpcChannel.ForAddress(localNode.Status.GrpcEndpoint, options);
        Channel = GrpcChannel.Intercept(authInterceptor);
    }

    public GrpcChannel GrpcChannel { get; set; }
    public CallInvoker Channel { get; set; }
}