using Microsoft.Extensions.Logging;

namespace Orbit.Util.Misc;

public static class AttemptUtil
{
    public static async Task<T> Attempt<T>(
        int maxAttempts = 5,
        long initialDelay = 1000,
        long maxDelay = long.MaxValue,
        double factor = 1.0,
        ILogger logger = null,
        Func<T> body = null)
    {
        var currentDelay = initialDelay;
        for (var i = 0; i < maxAttempts - 1; i++)
        {
            try
            {
                return body();
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, $"Attempt {i + 1}/{maxAttempts} failed. Retrying in {currentDelay} m.");
            }


            await Task.Delay(TimeSpan.FromMilliseconds(currentDelay));
            currentDelay = (long)(currentDelay * factor) < maxDelay ? (long)(currentDelay * factor) : maxDelay;
        }

        try
        {
            return body();
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, $"Attempt {maxAttempts}/{maxAttempts} failed. No more retries.");
            throw ex;
        }
    }
}