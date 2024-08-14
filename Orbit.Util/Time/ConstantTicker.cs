using Microsoft.Extensions.Logging;
using Orbit.Util.Concurrent;

namespace Orbit.Util.Time;

public class ConstantTicker
{
    private readonly Clock _clock;
    private readonly Action<Exception> _exceptionHandler;
    private readonly ILogger _logger;
    private readonly Func<Task> _onSlowTick;
    private readonly Func<Task> _onTick;
    private readonly long _targetTickRate;

    private readonly CancellationTokenSource _tokenSource;

    public ConstantTicker(
        long targetTickRate,
        Clock clock,
        ILogger logger = null,
        Action<Exception> exceptionHandler = null,
        bool autoStart = false,
        Func<Task> onTick = null,
        Func<Task> onSlowTick = null)
    {
        _tokenSource = new CancellationTokenSource();

        _targetTickRate = targetTickRate;
        _clock = clock;
        _logger = logger;
        _exceptionHandler = exceptionHandler;
        _onTick = onTick;
        _onSlowTick = onSlowTick;

        if (autoStart)
        {
            Start();
        }
    }

    public async Task Start()
    {
        //todo
        _logger?.LogTrace("Begin tick...");
        Task.Run(async () =>
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                var stopwatch = Stopwatch.Start(_clock);

                // try
                // {
                await _onTick();
                // }
                // catch (OperationCanceledException e)
                // {
                //     throw e;
                // }
                // catch (Exception ex)
                // {
                //     _exceptionHandler?.Invoke(ex);
                //     throw ex;
                // }

                var elapsed = stopwatch.Elapsed;
                var nextTickDelay = _targetTickRate - elapsed;

                if (elapsed > _targetTickRate)
                {
                    _logger?.LogWarning(
                        $"Slow tick. The application is unable to maintain its tick rate. Last tick took {elapsed} ms and the reference tick rate is {_targetTickRate} ms. The next tick will take place immediately.");
                    if (_onSlowTick != null)
                    {
                        await _onSlowTick();
                    }
                }

                _logger?.LogTrace($"Tick completed in {elapsed}ms. Next tick in {nextTickDelay}ms.");
                if (nextTickDelay > 0)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds((int)nextTickDelay), _tokenSource.Token);
                }
            }
        }, _tokenSource.Token);
    }

    public async Task Stop()
    {
        _tokenSource.Cancel();
        //await ticker;
        //WaitingForActivation
    }
}