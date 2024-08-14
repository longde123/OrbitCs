using System.Collections.Concurrent;

namespace Orbit.Util.Concurrent;

public class HashMapBackedAsyncMap<TK, TV> : BaseAsyncMap<TK, TV> where TV : class
{
    public virtual ConcurrentDictionary<TK, TV> Map { get; }

    public override async Task<TV?> Get(TK key)
    {
        if (Map.TryGetValue(key, out var value))
        {
            return value;
        }

        return null;
    }

    public override async Task<bool> Remove(TK key)
    {
        return Map.Remove(key, out _);
    }


    public override async Task<long> Count()
    {
        return Map.Count;
    }


    public override async Task<bool> CompareAndSet(TK key, TV? initialValue, TV? newValue)
    {
        if (initialValue == null)
        {
            Map.TryAdd(key, newValue);
        }
        else
        {
            Map.TryUpdate(key, newValue, initialValue);
        }

        return true;
    }
}