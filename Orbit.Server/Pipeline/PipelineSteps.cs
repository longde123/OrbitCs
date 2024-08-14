using Orbit.Server.Pipeline.Step;

namespace Orbit.Server.Pipeline;

public class PipelineSteps
{
    private readonly AuthStepIn _authStepIn;
    private readonly PlacementStepIn _placementStepIn;
    private readonly RoutingStepIn _routingStepIn;
    private readonly VerifyStepIn _verifyStepIn;

    private readonly IdentityStepOut _identityStepOut;
    private readonly RoutingStepOut _routingStepOut;
    private readonly TransportStepOut _transportStepOut;

    public PipelineSteps(
        IdentityStepOut identityStepOut,
        PlacementStepIn placementStepIn,
        VerifyStepIn verifyStepIn,
        RoutingStepIn routingStepIn,
        RoutingStepOut routingStepOut,
        AuthStepIn authStepIn,
        TransportStepOut transportStepOut)
    {
        _identityStepOut = identityStepOut;
        _placementStepIn = placementStepIn;
        _verifyStepIn = verifyStepIn;
        _routingStepIn = routingStepIn;
        _routingStepOut = routingStepOut;
        _authStepIn = authStepIn;
        _transportStepOut = transportStepOut;
    }

    public PipelineStepOut[] StepOuts => new PipelineStepOut[]
    {
        _identityStepOut, _routingStepOut, _transportStepOut
    };

    public PipelineStepIn[] StepIns => new PipelineStepIn[]
    {
        _authStepIn, _verifyStepIn, _placementStepIn, _routingStepIn
        // check cluster manager for node, pause if null and reintroduce to pipeline (log backoff) 
    };
}