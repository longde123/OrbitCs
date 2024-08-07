using Orbit.Util.Time;

namespace Orbit.Shared.Mesh;

public class NodeLease : IEquatable<NodeLease>
{
    public NodeLease()
    {
    }

    public NodeLease(string challengeToken, Timestamp expiresAt, Timestamp renewAt)
    {
        ChallengeToken = challengeToken;
        ExpiresAt = expiresAt;
        RenewAt = renewAt;
    }

    public string ChallengeToken { get; set; }
    public Timestamp ExpiresAt { get; set; }
    public Timestamp RenewAt { get; set; }

    public bool Equals(NodeLease? other)
    {
        //todo
        return other != null
               && ChallengeToken != null
               && ChallengeToken.Equals(other.ChallengeToken)
               && ExpiresAt.Equals(other.ExpiresAt)
               && RenewAt.Equals(other.RenewAt);
    }


    public override bool Equals(object? obj)
    {
        if (obj != null && obj is NodeLease nid)
        {
            return Equals(nid);
        }

        return false;
    }
}