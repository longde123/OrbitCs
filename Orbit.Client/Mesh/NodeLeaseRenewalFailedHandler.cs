/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using Microsoft.Extensions.Logging;
using Orbit.Client.Execution;
using Orbit.Util.Concurrent;
using Orbit.Util.Di;

namespace Orbit.Client.Mesh;

public interface INodeLeaseRenewalFailedHandler
{
    void OnLeaseRenewalFailed();
}

public class RestartOnNodeRenewalFailure : INodeLeaseRenewalFailedHandler
{
    private readonly ILogger _logger;
    private readonly OrbitClient _orbitClient;

    public RestartOnNodeRenewalFailure(OrbitClient orbitClient, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<RestartOnNodeRenewalFailure>();
        _orbitClient = orbitClient;
    }

    public void OnLeaseRenewalFailed()
    {
        _logger.LogInformation($"Beginning Orbit restart, node {_orbitClient.NodeId?.Key}");
        Task.Run(async () =>
        {
            await _orbitClient.Stop(new AddressableDeactivator.Instant());
            await _orbitClient.Start();
        }).Wait();
        _logger.LogInformation($"Orbit restart complete, node {_orbitClient.NodeId?.Key}");
    }

    public class RestartOnNodeRenewalFailureSingleton : ExternallyConfigured<INodeLeaseRenewalFailedHandler>
    {
        public override Type InstanceType => typeof(RestartOnNodeRenewalFailure);
    }
}

public class NodeLeaseRenewalFailed : Exception
{
    public NodeLeaseRenewalFailed(string message) : base(message)
    {
    }
}