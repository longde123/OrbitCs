namespace Orbit.Client.Util;

public class RemoteException : Exception
{
    public RemoteException(string msg) : base(msg)
    {
    }
}

public class NoSuchMethodException : Exception
{
    public NoSuchMethodException(string msg) : base(msg)
    {
    }
}

public class InvocationTargetException : Exception
{
    public InvocationTargetException(string msg) : base(msg)
    {
    }
}

public class TimeoutException : Exception
{
    public TimeoutException(string msg) : base(msg)
    {
    }
}