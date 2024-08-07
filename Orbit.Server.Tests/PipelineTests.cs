/*
 Copyright (C) 2015 - 2020 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Orbit.Server.Net;
using Orbit.Server.Pipeline;
using Orbit.Server.Pipeline.Step;
using Orbit.Shared.Mesh;
using Orbit.Shared.Net;
using Orbit.Shared.Router;

namespace Orbit.Server;

public class PipelineTests : BaseServerTest
{
    [Test]
    public async Task WhenANodeIsNotPresentInCluster_MessageIsSentToPipelineWithAttemptsIncremented()
    {
        var testNode = new NodeId("test", "test");
        var message = new Message
        {
            Content = Mock.Of<MessageContent.InvocationRequest>(),
            Target = new MessageTarget.Unicast(testNode)
        };

        var router = Mock.Of<Router.Router>();
        Mock.Get(router)
            .Setup(r => r.FindRoute(testNode, null))
            .Returns(new Route(new List<NodeId>()));

        var routingStep = new RoutingStep(router, new OrbitServerConfig());


        var context = Mock.Of<PipelineContext>();

        await routingStep.OnOutbound(context, message);

        Mock.Get(context).Verify(c => c.Next(It.IsAny<Message>()), Times.Never);
        Mock.Get(context)
            .Verify(c => c.PushNew(It.Is<Message>(m => m.Attempts == 1L), null));
    }

    [Test]
    public async Task WhenANodeIsPresentInCluster_MessageIsSentToNextStep()
    {
        var testNode = new NodeId("test", "test");
        var route = new Route(new List<NodeId> { testNode });
        var message = new Message
        {
            Content = Mock.Of<MessageContent.InvocationRequest>(),
            Target = new MessageTarget.Unicast(testNode)
        };

        var router = Mock.Of<Router.Router>();
        Mock.Get(router).Setup(r => r.FindRoute(testNode, null)).Returns(route);

        var routingStep = new RoutingStep(router, new OrbitServerConfig());

        var context = Mock.Of<PipelineContext>();
        await routingStep.OnOutbound(context, message);

        Expression<Func<Message, bool>> isSameRoute = m => route.Equals(((MessageTarget.RoutedUnicast)m.Target).Route);
        Mock.Get(context).Verify(c => c.Next(It.Is(isSameRoute)));
        Mock.Get(context).Verify(c => c.PushNew(It.IsAny<Message>(), It.IsAny<MessageMetadata>()), Times.Never);
    }

    [Test]
    public async Task WhenAMessageExceedsRetryAttempts_ErrorMessageIsSentToPipeline()
    {
        var testNode = new NodeId("test", "test");
        var sourceNode = new NodeId("source", "test");
        var message = new Message
        {
            Content = Mock.Of<MessageContent.InvocationRequest>(),
            Target = new MessageTarget.Unicast(testNode),
            Source = sourceNode,
            Attempts = 11
        };


        var router = Mock.Of<Router.Router>();
        Mock.Get(router).Setup(r => r.FindRoute(testNode, null)).Returns(new Route(new List<NodeId>()));

        var routingStep = new RoutingStep(router, new OrbitServerConfig());

        var context = Mock.Of<PipelineContext>();
        await routingStep.OnOutbound(context, message);

        Mock.Get(context).Verify(c => c.Next(It.IsAny<Message>()), Times.Never);
        Mock.Get(context).Verify(c =>
            c.PushNew(It.Is<Message>(m => m.Content is MessageContent.Error), It.IsAny<MessageMetadata>()));
    }

    [Test]
    public async Task WhenAnErrorMessageExceedsRetryAttempts_MessageIsDiscarded()
    {
        var testNode = new NodeId("test", "test");
        var sourceNode = new NodeId("source", "test");
        var message = new Message
        {
            Content = Mock.Of<MessageContent.Error>(),
            Target = new MessageTarget.Unicast(testNode),
            Source = sourceNode,
            Attempts = 11
        };

        var router = Mock.Of<Router.Router>();
        Mock.Get(router).Setup(r => r.FindRoute(testNode, null)).Returns(new Route(new List<NodeId>()));

        var routingStep = new RoutingStep(router, new OrbitServerConfig());

        var context = Mock.Of<PipelineContext>();
        await routingStep.OnOutbound(context, message);

        Mock.Get(context).Verify(c => c.Next(It.IsAny<Message>()), Times.Never);
        Mock.Get(context).Verify(c => c.PushNew(It.IsAny<Message>(), It.IsAny<MessageMetadata>()), Times.Never);
    }
}