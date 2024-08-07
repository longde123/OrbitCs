namespace Orbit.Util.Concurrent;

public class AtomicReference<T>
{
    private readonly object _lock = new();
    private T? _value;

    public AtomicReference(T? value)
    {
        _value = value;
    }


    public bool CompareAndSet(T? initValue, T newValue)
    {
        lock (_lock)
        {
            if (initValue == null || initValue.Equals(_value))
            {
                _value = newValue;
                return true;
            }
        }

        return false;
    }

    public T? Get()
    {
        return _value;
    }
}