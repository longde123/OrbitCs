using Orbit.Server.Pipeline.Step;

namespace Orbit.Server.Pipeline;

public class PipelineSteps
{
    private readonly AuthStep _authStep;
    private readonly IdentityStep _identityStep;
    private readonly PlacementStep _placementStep;
    private readonly RoutingStep _routingStep;
    private readonly TransportStep _transportStep;
    private readonly VerifyStep _verifyStep;

    public PipelineSteps(
        IdentityStep identityStep,
        PlacementStep placementStep,
        VerifyStep verifyStep,
        RoutingStep routingStep,
        AuthStep authStep,
        TransportStep transportStep)
    {
        this._identityStep = identityStep;
        this._placementStep = placementStep;
        this._verifyStep = verifyStep;
        this._routingStep = routingStep;
        this._authStep = authStep;
        this._transportStep = transportStep;
    }

    public PipelineStep[] Steps => new PipelineStep[]
    {
        _identityStep,
        _routingStep,
        // check cluster manager for node, pause if null and reintroduce to pipeline (log backoff)
        _placementStep,
        _verifyStep,
        _authStep, 
        _transportStep
    };
}