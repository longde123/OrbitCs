using Orbit.Server.Mesh;
using Orbit.Shared.Net;
using Orbit.Shared.OrException;

namespace Orbit.Server.Pipeline.Step;

public class VerifyStep : PipelineStep
{
    private readonly ClusterManager _clusterManager;

    public VerifyStep(ClusterManager clusterManager)
    {
        this._clusterManager = clusterManager;
    }

    public override async Task OnInbound(PipelineContext context, Message msg)
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

        await context.Next(msg);
    }
}