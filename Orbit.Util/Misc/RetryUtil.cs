namespace Orbit.Util.Misc;

public class RetriesExceededException : Exception
{
    public RetriesExceededException(string message) : base(message)
    {
    }
}

public static class RetryUtil
{
    public static async Task<T> Retry<T>(
        uint retryDelay = 1,
        int attempts = int.MaxValue,
        Func<Task<T>> body = null)
    {
        var remaining = attempts;
        while (remaining-- > 0)
        {
            try
            {
                var result = await body.Invoke();
                return result;
            }
            catch (Exception e)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(retryDelay));
            }
        }

        throw new RetriesExceededException($"Failed operation after {attempts} attempts");
    }
}