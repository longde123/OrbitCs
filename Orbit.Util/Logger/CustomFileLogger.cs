using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Orbit.Util.Logger;

// Customized ILoggerProvider, writes logs to text files
public class CustomFileLoggerProvider : ILoggerProvider
{
    private readonly string _logFileWriter;
    private readonly BlockingCollection<string> _dataQueue = new();
    private readonly CancellationTokenSource _tokenSource = new();

    public CustomFileLoggerProvider(string path)
    {
        _logFileWriter = path;
        Task.Run(() =>
        {
            using (var sw = File.AppendText(_logFileWriter))
            {
                foreach (var message in _dataQueue.GetConsumingEnumerable())
                {
                    // Get the formatted log message

                    // Append text to the file

                    sw.WriteLine(message);
                }
            }
        }, _tokenSource.Token);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new CustomFileLogger(categoryName, _dataQueue);
    }

    public void Dispose()
    {
        _tokenSource.Cancel();
    }
}

// Customized ILogger, writes logs to text files
public class CustomFileLogger : ILogger
{
    private readonly string _categoryName;
    private readonly BlockingCollection<string> _dataQueue;

    public CustomFileLogger(string categoryName, BlockingCollection<string> dataQueue)
    {
        _categoryName = categoryName;
        this._dataQueue = dataQueue;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
        // Ensure that only information level and higher logs are recorded
        return logLevel >= LogLevel.Information;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        // Ensure that only information level and higher logs are recorded
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = $"[{DateTime.Now.ToUniversalTime().ToString("ddThh:mm:ss.fffZ")}] {formatter(state, exception)}";
        _dataQueue.Add(message);
    }
}