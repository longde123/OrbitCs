using Grpc.Core;
using Orbit.Server.Concurrent;
using Orbit.Server.Net;
using Orbit.Shared.Mesh;
using Orbit.Shared.Proto;

namespace Orbit.Server.Service;

public class ConnectionService : Connection.ConnectionBase
{
    private readonly ConnectionManager _connectionManager;

    public ConnectionService(ConnectionManager connectionManager, RuntimeScopes runtimeScopes)
    {
        this._connectionManager = connectionManager;
    }


    public override async Task OpenStream(IAsyncStreamReader<MessageProto> requestStream,
        IServerStreamWriter<MessageProto> responseStream, ServerCallContext context)
    {
        var nodeId = (NodeId)context.UserState[ServerAuthInterceptor.NodeId];

        if (nodeId == null)
        {
            throw new ArgumentNullException("Node ID was not specified");
        }

        var cancellationToken = context.CancellationToken;


        await _connectionManager.OnNewClient(nodeId, requestStream, responseStream, cancellationToken);
    }
}