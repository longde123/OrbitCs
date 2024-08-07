namespace Orbit.Util.Concurrent;

public interface IAsyncMap<TK, TV>
{
    Task<TV?> Get(TK key);
    Task<TV> GetValue(TK key);
    Task<bool> Remove(TK key);
    Task<bool> CompareAndSet(TK key, TV? initialValue, TV? newValue);
    Task<long> Count();
    Task<TV> GetOrPut(TK key, Func<Task<TV>> block);
    Task<TV?> Manipulate(TK key, Func<TV?, TV?> block);
}

public abstract class BaseAsyncMap<TK, TV> : IAsyncMap<TK, TV> where TV : class
{
    public virtual async Task<TV?> Get(TK key)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<TV> GetValue(TK key)
    {
        var value = await Get(key);
        if (value != null)
        {
            return await Task.FromResult(value);
        }

        throw new NullReferenceException($"key {key} value null");
    }

    public virtual async Task<bool> Remove(TK key)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<bool> CompareAndSet(TK key, TV? initialValue, TV? newValue)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<long> Count()
    {
        throw new NotImplementedException();
    }

    public async Task<TV> GetOrPut(TK key, Func<Task<TV>> block)
    {
        var initial = await Get(key);
        if (initial != null)
        {
            return initial;
        }

        var computed = await block();

        var compareAndSet = await CompareAndSet(key, null, computed);
        if (compareAndSet)
        {
            return computed;
        }

        return await GetValue(key);
    }

    public async Task<TV?> Manipulate(TK key, Func<TV?, TV?> block)
    {
        var initialValue = await Get(key);
        var newValue = block(initialValue);
        var compareAndSet = await CompareAndSet(key, initialValue, newValue);
        if (compareAndSet)
        {
            return newValue;
        }

        return await Manipulate(key, block);
    }
}