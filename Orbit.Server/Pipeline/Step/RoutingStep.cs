using Orbit.Server.Service;
using Orbit.Shared.Net;

namespace Orbit.Server.Pipeline.Step;

public class RoutingStep : PipelineStep
{
    private readonly OrbitServerConfig _config;

    private readonly Meters.MeterCounter _retryAttempts;
    private readonly Meters.MeterCounter _retryErrors;

    private readonly Router.Router _router;

    public RoutingStep(Router.Router router, OrbitServerConfig config)
    {
        this._router = router;
        this._config = config;
        _retryAttempts = Meters.Counter(Meters.Names.RetryAttempts);
        _retryErrors = Meters.Counter(Meters.Names.RetryErrors);
    }

    public override async Task OnOutbound(PipelineContext context, Message msg)
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

        if (route == null)
        {
            throw new Exception("Could not find route for " + msg);
        }

        if (!route.IsValid())
        {
            Retry(context, msg);
            return;
        }

        //todo
        var newMsg = msg;
        newMsg.Target = new MessageTarget.RoutedUnicast(route);

        await context.Next(newMsg);
    }

    public override async Task OnInbound(PipelineContext context, Message msg)
    {
        if (msg.Target == null)
        {
            throw new Exception("Node target was not resolved");
        }

        context.PushNew(msg);
    }

    public void Retry(PipelineContext context, Message msg)
    {
        Message nextMsg = null;

        if (msg.Attempts < _config.MessageRetryAttempts)
        {
            _retryAttempts.Increment();
            nextMsg = msg;
            nextMsg.Attempts = msg.Attempts + 1;
        }
        else if (!(msg.Content is MessageContent.Error))
        {
            _retryErrors.Increment();
            nextMsg = msg;

            nextMsg.Content = new MessageContent.Error($"Failed to deliver message after {msg.Attempts} attempts");
            nextMsg.Target = new MessageTarget.Unicast(msg.Source);
            nextMsg.Attempts = 0;
        }

        if (nextMsg != null)
        {
            context.PushNew(nextMsg);
        }
    }
}