using Orbit.Server.Net;
using Orbit.Server.Service;
using Orbit.Shared.Net;
using Orbit.Shared.Router;

namespace Orbit.Server.Pipeline.Step;

public class RoutingStepOut : PipelineStepOut
{
    private readonly OrbitServerConfig _config;

    private readonly Meters.MeterCounter _retryAttempts;
    private readonly Meters.MeterCounter _retryErrors;
    private readonly RemoteMeshNodeManager? _remoteMeshNodeManager;
    private readonly Router.Router _router;


    public RoutingStepOut(Router.Router router, OrbitServerConfig config, RemoteMeshNodeManager? remoteMeshNodeManager)
    {
        _router = router;
        _config = config;
        _remoteMeshNodeManager = remoteMeshNodeManager;
        _retryAttempts = Meters.Counter(Meters.Names.RetryAttempts);
        _retryErrors = Meters.Counter(Meters.Names.RetryErrors);
    }

    public override async Task<bool> OnOutbound(PipelineContext context, Message msg)
    {
        if (msg.Target == null)
        {
            throw new ArgumentNullException("Target may not be null for outbound messages. " + msg);
        }

        var route = TryFindRoute(msg);
        if (route == null)
        {
            throw new Exception("Could not find route for " + msg);
        }

        while (route != null && !route.IsValid() && msg.Attempts < _config.MessageRetryAttempts)
        {
            _retryAttempts.Increment();
            msg.Attempts = msg.Attempts + 1;
            if (_remoteMeshNodeManager != null)
            {
                await _remoteMeshNodeManager.Tick();
            }

            route = TryFindRoute(msg);
        }

        if (route != null && !route.IsValid())
        {
            if (!(msg.Content is MessageContent.Error))
            {
                _retryErrors.Increment();
                msg.Content = new MessageContent.Error($"Failed to deliver message after {msg.Attempts} attempts");
                msg.Target = new MessageTarget.Unicast(msg.Source);
                msg.Attempts = 0;
            }
        }

        var newMsg = msg;
        newMsg.Target = new MessageTarget.RoutedUnicast(route);

        return true;
    }

    private Route? TryFindRoute(Message msg)
    {
        if (msg.Target == null)
        {
            throw new ArgumentNullException("Target may not be null for outbound messages. " + msg);
        }

        var route = msg.Target switch
        {
            MessageTarget.Unicast target => _router.FindRoute(target.TargetNode),
            MessageTarget.RoutedUnicast target => _router.FindRoute(target.Route.Pop().NodeId, target.Route),
            _ => null
        };
        return route;
    }
}