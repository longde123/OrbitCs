using Orbit.Client.Net;
using Orbit.Shared.Addressable;
using Orbit.Shared.Proto;

namespace Orbit.Client.Mesh;

public class AddressableLeaser
{
    private readonly AddressableManagement.AddressableManagementClient _addressableManagementStub;

    public AddressableLeaser(GrpcClient grpcClient)
    {
        _addressableManagementStub = new AddressableManagement.AddressableManagementClient(grpcClient.Channel);
    }

    public async Task<AddressableLease?> RenewLease(AddressableReference reference)
    {
        var request = new RenewAddressableLeaseRequestProto
        {
            Reference = reference.ToAddressableReferenceProto()
        };

        var response = await _addressableManagementStub.RenewLeaseAsync(request);
        return response?.Lease?.ToAddressableLease();
    }

    public async Task<bool> AbandonLease(AddressableReference reference)
    {
        var request = new AbandonAddressableLeaseRequestProto
        {
            Reference = reference.ToAddressableReferenceProto()
        };

        var response = await _addressableManagementStub.AbandonLeaseAsync(request);
        return response?.Abandoned ?? false;
    }
}