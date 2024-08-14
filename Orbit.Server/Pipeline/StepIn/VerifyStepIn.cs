using Orbit.Server.Mesh;
using Orbit.Shared.Net;
using Orbit.Shared.OrException;

namespace Orbit.Server.Pipeline.Step;

public class VerifyStepIn : PipelineStepIn
{
    private readonly ClusterManager _clusterManager;

    public VerifyStepIn(ClusterManager clusterManager)
    {
        _clusterManager = clusterManager;
    }

    public override async Task<bool> OnInbound(PipelineContext context, Message msg)
    {
        var source = msg.Source;

        if (source == null)
        {
            throw new ArgumentNullException("Source should not be null at this point");
        }

        if (await _clusterManager.GetNode(source) == null)
        {
            throw new InvalidNodeId(source);
        }

        return true;
    }
}