using System.Collections.Concurrent;

namespace Orbit.Util.Concurrent;

public class HashMapBackedAsyncMap<TK, TV> : BaseAsyncMap<TK, TV> where TV : class
{
    public virtual ConcurrentDictionary<TK, TV> Map { get; }

    public override async Task<TV?> Get(TK key)
    {
        if (Map.TryGetValue(key, out var value))
        {
            return await Task.FromResult(value);
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
        if (Map.TryGetValue(key, out var oldValue))
        {
            if (initialValue != oldValue)
            {
                return false;
                //  throw new Exception("Could not map nitialValue!=oldValue");
            }
        }

        Map[key] = newValue;
        return true;
    }
}