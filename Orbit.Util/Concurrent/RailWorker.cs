using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;

namespace Orbit.Util.Concurrent;

public class RailWorker<T>
{
    private readonly BlockingCollection<T> _channel;
    private readonly ILogger _logger;
    private readonly List<Task> _workers;
    private bool _autoStart = false;
    private readonly Func<T, Task> _onMessage;
    private readonly int _railCount;
    private readonly CancellationTokenSource _tokenSource;

    public RailWorker(int buffer = 10000, int railCount = 128, ILogger logger = null, bool autoStart = false,
        Func<T, Task> onMessage = null)
    {
        _tokenSource = new CancellationTokenSource();
        _channel = new BlockingCollection<T>();
        _logger = logger;
        _workers = new List<Task>(); //[railCount];
        _railCount = railCount;
        _onMessage = onMessage;
        if (autoStart)
        {
            StartWorkers();
        }
    }

    public bool Offer(T msg)
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("Rail worker is not initialized.");
        }

        _channel.Add(msg);
        return true;
    }

    public void StartWorkers()
    {
        for (var i = 0; i < _railCount; i++)
        {
            var task = Task.Run(async () =>
            {
                foreach (var msg in _channel.GetConsumingEnumerable())
                {
                    _onMessage(msg);
                    // }
                    // catch (Exception e)
                    // {
                    //     _logger?.LogWarning($"Error: Exception caught in rail worker {e}");
                    // }
                }
            }, _tokenSource.Token);
            _workers.Add(task);
        }

        _logger?.LogInformation($"Started a rail worker with {_workers.Count} rails and a {_channel} entry buffer.");
    }

    public async Task StopWorkers()
    {
        _tokenSource.Cancel();
        _workers.Clear();
        _channel.CompleteAdding();
        // await Task.WhenAll(_workers);
    }
}