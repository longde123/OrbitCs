namespace Orbit.Server.Mesh;

public class LeaseDuration
{
    public LeaseDuration(long leaseDuration)
    {
        Duration = leaseDuration;
    }

    public long Duration { get; }

    public TimeSpan ExpiresIn => TimeSpan.FromSeconds(Duration);

    public TimeSpan RenewIn => TimeSpan.FromSeconds(Duration / 2);
}