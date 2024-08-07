using Orbit.Shared.Addressable;

namespace Orbit.Shared.Proto;

public static class AddressableLeaseExtensions
{
    public static RenewAddressableLeaseResponseProto ToAddressableLeaseResponseProto(
        this AddressableLease addressableLease)
    {
        var renewAddressableLeaseResponseProto = new RenewAddressableLeaseResponseProto
        {
            Status = RenewAddressableLeaseResponseProto.Types.Status.Ok,
            Lease = addressableLease.ToAddressableLeaseProto()
        };
        return renewAddressableLeaseResponseProto;
    }

    public static RenewAddressableLeaseResponseProto ToAddressableLeaseResponseProto(this Exception exception)
    {
        return new RenewAddressableLeaseResponseProto
        {
            Status = RenewAddressableLeaseResponseProto.Types.Status.Error,
            ErrorDescription = exception.ToString()
        };
    }
}