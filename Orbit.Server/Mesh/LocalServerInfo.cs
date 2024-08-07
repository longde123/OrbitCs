namespace Orbit.Server.Mesh;

public class LocalServerInfo
{
    public LocalServerInfo()
    {
    }

    public LocalServerInfo(string url, int port)
    {
        Port = port;
        Url = url;
    }

    public int Port { get; set; }
    public string Url { get; set; }
}