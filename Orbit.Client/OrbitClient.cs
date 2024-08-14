using Microsoft.Extensions.Logging;
using Orbit.Client.Actor;
using Orbit.Client.Addressable;
using Orbit.Client.Execution;
using Orbit.Client.Mesh;
using Orbit.Client.Net;
using Orbit.Client.Util;
using Orbit.Shared.Mesh;
using Orbit.Util.Concurrent;
using Orbit.Util.Di;
using Orbit.Util.Logger;
using Orbit.Util.Misc;
using Orbit.Util.Time;

namespace Orbit.Client;

public class OrbitClient
{
    private readonly CapabilitiesScanner _capabilitiesScanner;
    private readonly Clock _clock;
    private readonly ConnectionHandler _connectionHandler;
    private readonly ComponentContainer _container;
    private readonly AddressableDefinitionDirectory _definitionDirectory;
    private readonly ExecutionSystem _executionSystem;
    private readonly LocalNode _localNode;
    private readonly ILogger _logger;
    private readonly MessageHandler _messageHandler;
    private readonly NodeLeaser _nodeLeaser;
    private readonly INodeLeaseRenewalFailedHandler _nodeLeaseRenewalFailedHandler;
    private readonly ConstantTicker _ticker;
    public ActorProxyFactory ActorFactory { get; }

    public OrbitClientConfig Config { get; }

    public ClientState Status => _localNode.Status.ClientState;

    public NodeId? NodeId => _localNode.Status.NodeInfo?.Id;

    public OrbitClient(OrbitClientConfig config)
    {
        Config = config;
        _container = new ComponentContainer();
        _clock = config.Clock;

        //Create an ILoggerFactory
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            if (!Directory.Exists("log"))
                Directory.CreateDirectory("log");
            var logFilePath = $"log/console_client_log_{DateTime.UtcNow.ToString("_yyyy_MM_ddTHH_mm_ss_fffZ")}.txt";

            builder.SetMinimumLevel(LogLevel.Trace); // 设置最低日志级别 
            //Add a custom log provider to write logs to text files
            builder.AddProvider(new CustomFileLoggerProvider(logFilePath));
        });
        _logger = loggerFactory.CreateLogger<OrbitClient>();
        _container.Configure(containerRoot =>
        {
            containerRoot.Instance(loggerFactory);
            containerRoot.Instance(this);
            containerRoot.Instance(config);
            containerRoot.Instance(_clock);
            containerRoot.Instance(new LocalNode(config));

            containerRoot.Singleton<GrpcClient>();
            containerRoot.Singleton<ClientAuthInterceptor>();
            containerRoot.Singleton<ConnectionHandler>();
            containerRoot.Singleton<MessageHandler>();

            containerRoot.Singleton<NodeLeaser>();
            containerRoot.Singleton<AddressableLeaser>();
            containerRoot.ExternallyConfigured(config.NodeLeaseRenewalFailedHandler);

            containerRoot.Singleton<Serializer.Serializer>();

            containerRoot.Singleton<CapabilitiesScanner>();
            containerRoot.Singleton<AddressableProxyFactory>();
            containerRoot.Singleton<InvocationSystem>();
            containerRoot.Singleton<AddressableDefinitionDirectory>();
            containerRoot.ExternallyConfigured(config.AddressableConstructor);
            containerRoot.ExternallyConfigured(config.AddressableDeactivator);

            containerRoot.Singleton<ExecutionSystem>();
            containerRoot.Singleton<ExecutionLeases>();

            containerRoot.Singleton<ActorProxyFactory>();

            config.ContainerOverrides(containerRoot);
        });

        _nodeLeaser = _container.Inject<NodeLeaser>().Value;
        _messageHandler = _container.Inject<MessageHandler>().Value;
        _connectionHandler = _container.Inject<ConnectionHandler>().Value;
        _capabilitiesScanner = _container.Inject<CapabilitiesScanner>().Value;
        _localNode = _container.Inject<LocalNode>().Value;
        _definitionDirectory = _container.Inject<AddressableDefinitionDirectory>().Value;
        _executionSystem = _container.Inject<ExecutionSystem>().Value;
        _nodeLeaseRenewalFailedHandler = _container.Inject<INodeLeaseRenewalFailedHandler>().Value;
        ActorFactory = _container.Inject<ActorProxyFactory>().Value;


        _ticker = new ConstantTicker((long)config.TickRate.TotalMilliseconds, _clock, _logger, OnUnhandledException,
            false, Tick);
    }


    public async Task Start()
    {
        //scope.Launch(

        _logger.LogInformation($"Starting Orbit client - Node: {NodeId}");
        var stopwatch = Stopwatch.Start(_clock);
        {
            _localNode.Manipulate(node =>
            {
                node.ClientState = ClientState.Connecting;
                return node;
            });

            _capabilitiesScanner.Scan();
            _definitionDirectory.SetupDefinition(_capabilitiesScanner.AddressableInterfaces,
                _capabilitiesScanner.InterfaceLookup);
            _localNode.Manipulate(node =>
            {
                node.Capabilities = _definitionDirectory.GenerateCapabilities();
                return node;
            });

            try
            {
                await RetryUtil.Retry(1, 5, async () =>
                {
                    await _nodeLeaser.JoinCluster();
                    return 0;
                });
            }
            catch (RetriesExceededException ex)
            {
                _logger.LogInformation("Failed to join cluster");
                _localNode.Reset();
                throw new RemoteException("Failed to join cluster");
            }

            _connectionHandler.Connect();

            _localNode.Manipulate(node =>
            {
                node.ClientState = ClientState.Connected;
                return node;
            });

            await _ticker.Start();
        }
        var elapsed = stopwatch.Elapsed;
        _logger.LogInformation($"Orbit client started successfully in {elapsed}ms.");
    }

    private async Task Tick()
    {
        _connectionHandler.Tick();
        await _nodeLeaser.Tick();
        _messageHandler.Tick();
        await _executionSystem.Tick();
    }

    public async Task Stop(AddressableDeactivator? deactivator = null)
    {
        //scope.Launch(

        _logger.LogInformation($"Stopping Orbit node {NodeId}...");
        var stopwatch = Stopwatch.Start(_clock);
        {
            _localNode.Manipulate(node =>
            {
                node.ClientState = ClientState.Stopping;
                return node;
            });

            try
            {
                await _nodeLeaser.LeaveCluster();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Encountered a server error while leaving cluster {ex.Message}");
            }

            await _executionSystem.Stop(deactivator);
            await _ticker.Stop();
            await _connectionHandler.Disconnect();
            _localNode.Reset();
        }
        ;
        var elapsed = stopwatch.Elapsed;
        _logger.LogInformation($"Orbit stopped successfully in {elapsed}ms.");
    }

    private void OnUnhandledException(Exception ex)
    {
        switch (ex)
        {
            case NodeLeaseRenewalFailed _:
                _logger.LogError("Node lease renewal failed...");
                if (Status == ClientState.Connected)
                {
                    _localNode.Manipulate(node =>
                    {
                        node.ClientState = ClientState.Stopping;
                        return node;
                    });
                    _nodeLeaseRenewalFailedHandler.OnLeaseRenewalFailed();
                }

                break;
            default:
                _logger.LogError(ex, "Unhandled exception in Orbit Client.");
                break;
        }
    }
}