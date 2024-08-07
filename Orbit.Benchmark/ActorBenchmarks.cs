using BenchmarkDotNet.Attributes;
using Orbit.Client;
using Orbit.Client.Actor;
using Orbit.Server;
using Orbit.Server.Mesh;

namespace Orbit.Benchmark;

internal interface IBasicBenchmarkActor : IActorWithInt32Key
{
    Task<string> Echo(string msg);
}

public class BasicBenchmarkActorImpl : AbstractActor, IBasicBenchmarkActor
{
    public async Task<string> Echo(string msg)
    {
        return await Task.FromResult(msg);
    }
}

[MemoryDiagnoser]
[RPlotExporter]
[CsvMeasurementsExporter]
[JsonExporter]
[MinColumn]
[MaxColumn]
[MeanColumn]
[MedianColumn]
[HtmlExporter]
[SimpleJob(1, 5, invocationCount: 20)]
public class ActorBenchmarks
{
    private const int RequestsPerBatch = 500;
    private const int ActorPool = 1000;
    private List<IBasicBenchmarkActor> _actors;
    private OrbitClient _client;
    private readonly string _nameSpace = "benchmarks";

    private readonly int _port = 5899;

    private OrbitServer _server;
    private readonly string _targetUri = "https://localhost:5899/";

    [GlobalSetup]
    public void Setup()
    {
        var serverConfig = new OrbitServerConfig
        {
            ServerInfo = new LocalServerInfo(_targetUri, _port)
        };
        var clientConfig = new OrbitClientConfig
        {
            GrpcEndpoint = _targetUri,
            Namespace = _nameSpace,
            Packages = new List<string> { "Orbit.Benchmark" }
        };

        _server = new OrbitServer(serverConfig);
        _client = new OrbitClient(clientConfig);

        _server.Start().Wait();
        _client.Start().Wait();

        _actors = new List<IBasicBenchmarkActor>();
        for (var i = 0; i < ActorPool; i++)
        {
            var actor = _client.ActorFactory.CreateProxy<IBasicBenchmarkActor>(i);
            actor.Echo("Chevron " + i + " encoded...").Wait();
            _actors.Add(actor);
        }
    }

    [Benchmark]
    // [Threading(8)]
    // [OperationsPerInvoke(REQUESTS_PER_BATCH)]
    public void EchoThroughputBenchmark()
    {
        BatchIteration();
    }

    [Benchmark]
    // [Threading(8)]
    // [OperationsPerInvoke(REQUESTS_PER_BATCH)]
    // [BenchmarkMode(BenchmarkDotNet.Attributes.Modes.AverageTime)]
    // [BenchmarkDotNet.Attributes.Exporters.Csv.OutputTimeUnit(BenchmarkDotNet.Attributes.TimeUnit.Microsecond)]
    public void EchoTimingBenchmark()
    {
        BatchIteration();
    }

    private void BatchIteration()
    {
        var myList = new List<Task<string>>(RequestsPerBatch);
        for (var i = 0; i < RequestsPerBatch; i++)
        {
            var actor = _actors[new Random().Next(_actors.Count)];
            myList.Add(actor.Echo("Chevron " + i + " locked."));
        }

        Task.WhenAll(myList).Wait();
    }

    [GlobalCleanup]
    public void Teardown()
    {
        _client.Stop().Wait();
        _server.Stop().Wait();
    }
}