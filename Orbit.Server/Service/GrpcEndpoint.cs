using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orbit.Server.Mesh;

namespace Orbit.Server.Service;

public class GrpcEndpoint
{
    private readonly AddressableManagementService _addressableManagementService;
    private readonly ConnectionService _connectionService;
    private readonly HealthService _healthService;
    private readonly LocalServerInfo _localServerInfo;
    private readonly ILogger _logger;
    private readonly NodeManagementService _nodeManagementService;
    private readonly ServerAuthInterceptor _serverAuthInterceptor;
    private WebApplication? _serverTask;

    public GrpcEndpoint(ServerAuthInterceptor serverAuthInterceptor, HealthService healthService,
        NodeManagementService nodeManagementService, AddressableManagementService addressableManagementService,
        ConnectionService connectionService, LocalServerInfo localServerInfo, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<GrpcEndpoint>();
        this._serverAuthInterceptor = serverAuthInterceptor;
        this._healthService = healthService;
        this._nodeManagementService = nodeManagementService;
        this._addressableManagementService = addressableManagementService;
        this._connectionService = connectionService;
        this._localServerInfo = localServerInfo;
    }


    public void ConfigureServices(IServiceCollection server)
    {
        server.AddGrpc(options => { options.Interceptors.Add<ServerAuthInterceptor>(); });

        server.AddSingleton(_serverAuthInterceptor);
        server.Add(new ServiceDescriptor(typeof(HealthService), _healthService));
        server.Add(new ServiceDescriptor(typeof(NodeManagementService), _nodeManagementService));
        server.Add(new ServiceDescriptor(typeof(AddressableManagementService), _addressableManagementService));
        server.Add(new ServiceDescriptor(typeof(ConnectionService), _connectionService));
    }

    public async Task Start()
    {
        _logger.LogInformation($"Starting gRPC Endpoint on {_localServerInfo.Port}...");

        var builder = WebApplication.CreateBuilder();
        ConfigureServices(builder.Services);
        builder.WebHost.UseUrls($"https://localhost:{_localServerInfo.Port}");
        var app = builder.Build();
        app.MapGrpcService<HealthService>();
        app.MapGrpcService<NodeManagementService>();
        app.MapGrpcService<AddressableManagementService>();
        app.MapGrpcService<ConnectionService>();
        app.MapGet("/", () => "Hello World!");
        _serverTask = app;
        app.RunAsync();


        _logger.LogInformation($"gRPC Endpoint started on {_localServerInfo.Port}.");
    }

    public async Task Stop()
    {
        await _serverTask?.StopAsync();
    }
}