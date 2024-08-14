using Microsoft.Extensions.Logging;
using Orbit.Server.Auth;
using Orbit.Server.Mesh;
using Orbit.Server.Net;
using Orbit.Server.Pipeline;
using Orbit.Server.Pipeline.Step;
using Orbit.Server.Service;
using Orbit.Shared.Mesh;
using Orbit.Util.Concurrent;
using Orbit.Util.Di;
using Orbit.Util.Logger;
using Orbit.Util.Time;

namespace Orbit.Server;

public class OrbitServer : IHealthCheck
{
    private readonly IAddressableDirectory _addressableDirectory;
    private readonly Clock _clock;
    private readonly ClusterManager _clusterManager;
    private readonly OrbitServerConfig _config;
    private readonly GrpcEndpoint _grpcEndpoint;
    private readonly LocalNodeInfo _localNodeInfo;
    private readonly ILogger _logger;
    private readonly INodeDirectory _nodeDirectory;
    private readonly Pipeline.Pipeline _pipeline;
    private readonly RemoteMeshNodeManager _remoteMeshNodeManager;
    private readonly AtomicReference<ShutdownLatch> _shutdownLatch;

    private readonly Meters.MeterCounter _slowTick;
    private readonly ConstantTicker _ticker;
    private readonly Meters.MeterTimer _tickTimer;

    public OrbitServer(OrbitServerConfig config)
    {
        _config = config;
        _shutdownLatch = new AtomicReference<ShutdownLatch>(new ShutdownLatch());
        Container = new ComponentContainer();
        _clock = config.Clock;

        //Create an ILoggerFactory
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            if (!Directory.Exists("log"))
                Directory.CreateDirectory("log");
            var logFilePath = $"log/console_server_log_{DateTime.UtcNow.ToString("_yyyy_MM_ddTHH_mm_ss_fffZ")}.txt";

            builder.SetMinimumLevel(LogLevel.Trace); // 设置最低日志级别
            // builder.AddLog4Net(logFilePath);
            //Add a custom log provider to write logs to text files
            builder.AddProvider(new CustomFileLoggerProvider(logFilePath));
        });


        // ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        // {
        //     builder
        //         .AddFilter("Microsoft", LogLevel.Warning)
        //         .AddFilter("System", LogLevel.Warning) 
        //         .AddFilter("Orbit.Server", LogLevel.None)
        //         .AddConsole();
        // });
        _logger = loggerFactory.CreateLogger<OrbitServer>();
        Container.Configure(containerRoot =>
        {
            containerRoot.Instance(loggerFactory);
            containerRoot.Instance(this);
            containerRoot.Instance(config);
            containerRoot.Instance(_clock);
            containerRoot.Instance(config.ServerInfo);

            // Service
            containerRoot.Singleton<GrpcEndpoint>();
            containerRoot.Singleton<ServerAuthInterceptor>();
            containerRoot.Singleton<NodeManagementService>();
            containerRoot.Singleton<AddressableManagementService>();
            containerRoot.Singleton<ConnectionService>();
            containerRoot.Singleton<HealthCheckList>();
            containerRoot.Singleton<HealthService>();

            // Net
            containerRoot.Singleton<ConnectionManager>();

            // Pipeline
            containerRoot.Singleton<Pipeline.Pipeline>();
            containerRoot.Singleton<PipelineSteps>();
            containerRoot.Singleton<BlankStepIn>();
            containerRoot.Singleton<PlacementStepIn>();
            containerRoot.Singleton<IdentityStepOut>();
            containerRoot.Singleton<RoutingStepIn>();
            containerRoot.Singleton<EchoStepIn>();
            containerRoot.Singleton<VerifyStepIn>();
            containerRoot.Singleton<AuthStepIn>();
            containerRoot.Singleton<TransportStepOut>();

            // Mesh
            containerRoot.Singleton<LocalNodeInfo>();
            containerRoot.Singleton<ClusterManager>();
            containerRoot.Singleton<AddressableManager>();
            containerRoot.Singleton<RemoteMeshNodeManager>();
            containerRoot.ExternallyConfigured(config.NodeDirectory);
            containerRoot.ExternallyConfigured(config.AddressableDirectory);
            containerRoot.ExternallyConfigured(config.MeterRegistry);
            // Auth
            containerRoot.Singleton<AuthSystem>();

            // Router
            containerRoot.Singleton<Router.Router>();

            // Hook to allow overriding container definitions
            config.ContainerOverrides(containerRoot);
        });

        Meters.Init(Container.Inject<IMeterRegistry>().Value);
        _grpcEndpoint = Container.Inject<GrpcEndpoint>().Value;
        _localNodeInfo = Container.Inject<LocalNodeInfo>().Value;
        _clusterManager = Container.Inject<ClusterManager>().Value;
        _nodeDirectory = Container.Inject<INodeDirectory>().Value;
        _addressableDirectory = Container.Inject<IAddressableDirectory>().Value;
        _pipeline = Container.Inject<Pipeline.Pipeline>().Value;
        _remoteMeshNodeManager = Container.Inject<RemoteMeshNodeManager>().Value;

        _slowTick = Meters.Counter(Meters.Names.SlowTicks);
        _tickTimer = Meters.Timer(Meters.Names.TickTimer);
        _ticker = new ConstantTicker((long)config.TickRate.ToTimeSpan().TotalMilliseconds, _clock,
            _logger, OnUnhandledException, false, Tick, async () => { _slowTick.Increment(); }); // slowTick++);


        Meters.Gauge(Meters.Names.NodeCount, () =>
        {
            var count = _nodeDirectory.Count().GetAwaiter().GetResult();
            _logger.LogInformation($"Gauge NodeCount {count}");
            return (int)count;
        });
        Meters.Gauge(Meters.Names.AddressableCount, () =>
        {
            var count = _addressableDirectory.Count().GetAwaiter().GetResult();
            _logger.LogInformation($"Gauge AddressableCount {count}");
            return (int)count;
        });
    }

    public ComponentContainer Container { get; }

    private NodeStatus NodeStatus => _localNodeInfo.Info.NodeStatus;


    public async Task<bool> IsHealthy()
    {
        return NodeStatus == NodeStatus.Active;
    }

    public async Task Start()
    {
        //todo
        //await runtimeScopes.CpuScope.Launch(async () =>
        {
            _logger.LogInformation("Starting Orbit server...");
            _logger.LogInformation(
                $"Lease expirations: Addressable: {_config.AddressableLeaseDuration.Duration}s, Node: {_config.NodeLeaseDuration.Duration}s");
            var stopwatch = Stopwatch.Start(_clock);

            {
                _pipeline.Start();
                await _localNodeInfo.Start();
                await _ticker.Start();
                await _grpcEndpoint.Start();
                await _localNodeInfo.UpdateInfo(info =>
                {
                    info.NodeStatus = NodeStatus.Active;
                    return info;
                });
                if (_config.AcquireShutdownLatch)
                {
                    var latch = new ShutdownLatch();
                    _shutdownLatch.AtomicSet(s => latch);
                    latch.Acquire();
                }
            }
            ;
            var elapsed = stopwatch.Elapsed;
            _logger.LogInformation($"Orbit server started successfully in {elapsed}ms.");
        }
        //);
    }

    public async Task Stop()
    {
        //todo
        //await runtimeScopes.CpuScope.Launch(async () =>
        {
            _logger.LogInformation("Stopping Orbit server...");
            var stopwatch = Stopwatch.Start(_clock);
            {
                await _localNodeInfo.UpdateInfo(info =>
                {
                    info.NodeStatus = NodeStatus.Draining;
                    return info;
                });
                await _grpcEndpoint.Stop();
                await _ticker.Stop();
                await _pipeline.Stop();
                await _localNodeInfo.UpdateInfo(info =>
                {
                    info.NodeStatus = NodeStatus.Stopped;
                    return info;
                });
                await _nodeDirectory.Remove(_localNodeInfo.Info.Id);
                _shutdownLatch.Get()?.Release();
                _shutdownLatch.AtomicSet(s => null);
            }
            var elapsed = stopwatch.Elapsed;
            _logger.LogInformation($"Orbit server stopped successfully in {elapsed}ms.");
        } //);

        Meters.Clear();
    }

    public async Task Tick()
    {
        //todo
        //   await _tickTimer.Record(async () =>
        //{
        await _localNodeInfo.Tick();
        await _clusterManager.Tick();
        await _nodeDirectory.Tick();
        await _addressableDirectory.Tick();
        await _remoteMeshNodeManager.Tick();
        //  return 1;
        // });
    }


    private void OnUnhandledException(Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception in Orbit.");
    }
}