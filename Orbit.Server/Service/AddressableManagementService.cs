using Grpc.Core;
using Orbit.Server.Mesh;
using Orbit.Shared.Mesh;
using Orbit.Shared.Proto;
using static Orbit.Server.Service.ServerAuthInterceptor;

namespace Orbit.Server.Service;

public class AddressableManagementService : AddressableManagement.AddressableManagementBase
{
    private readonly Meters.MeterTimer _addressableLeaseAbandonTimer;
    private readonly Meters.MeterTimer _addressableLeaseRenewalTimer;
    private readonly AddressableManager _addressableManager;

    public AddressableManagementService(AddressableManager addressableManager)
    {
        //todo
        //runtimeScopes.IoScope.CoroutineContext
        _addressableManager = addressableManager;
        _addressableLeaseRenewalTimer = Meters.Timer(Meters.Names.AddressableLeaseRenewalTimer);
        _addressableLeaseAbandonTimer = Meters.Timer(Meters.Names.AddressableLeaseAbandonTimer);
    }

    public override async Task<RenewAddressableLeaseResponseProto> RenewLease(RenewAddressableLeaseRequestProto request,
        ServerCallContext context)
    {
        // return await _addressableLeaseRenewalTimer.Record(async () =>
        {
            try
            {
                var nodeId = (NodeId)context.UserState[ServerAuthInterceptor.NodeId];
                var it = await _addressableManager.RenewLease(request.Reference.ToAddressableReference(), nodeId);
                return it.ToAddressableLeaseResponseProto();
            }
            catch (Exception t)
            {
                return t.ToAddressableLeaseResponseProto();
            }
        }
        // );
    }

    public override async Task<AbandonAddressableLeaseResponseProto> AbandonLease(
        AbandonAddressableLeaseRequestProto request, ServerCallContext context)
    {
        //  return await _addressableLeaseAbandonTimer.Record(async () =>
        {
            var nodeId = (NodeId)context.UserState[ServerAuthInterceptor.NodeId];
            var reference = request.Reference.ToAddressableReference();
            var result = false;

            try
            {
                result = await _addressableManager.AbandonLease(reference, nodeId);
            }
            catch (Exception t)
            {
                throw t;
            }

            return new AbandonAddressableLeaseResponseProto
            {
                Abandoned = result
            };
        }
        //  );
    }
}