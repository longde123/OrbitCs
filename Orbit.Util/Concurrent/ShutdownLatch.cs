namespace Orbit.Util.Concurrent;

public class ShutdownLatch
{
    //todo
    private readonly AutoResetEvent _latch = new(false);
    private Thread _thread;

    public void Acquire()
    {
        _thread = new Thread(() => { _latch.WaitOne(); });
        _thread.Name = "orbit-shutdown-latch";
        _thread.IsBackground = false;
        _thread.Start();
    }

    public void Release()
    {
        _latch.Set();
    }
}