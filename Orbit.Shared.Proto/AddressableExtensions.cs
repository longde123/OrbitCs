using Orbit.Shared.Addressable;

namespace Orbit.Shared.Proto;

public static class AddressableExtensions
{
    public static AddressableReferenceProto ToAddressableReferenceProto(this AddressableReference reference)
    {
        return new AddressableReferenceProto
        {
            Type = reference.Type,
            Key = reference.Key.ToAddressableKeyProto()
        };
    }

    public static AddressableReference ToAddressableReference(this AddressableReferenceProto proto)
    {
        return new AddressableReference
        {
            Type = proto.Type,
            Key = proto.Key.ToAddressableKey()
        };
    }

    public static AddressableKeyProto ToAddressableKeyProto(this Key key)
    {
        var builder = new AddressableKeyProto();
        switch (key)
        {
            case Key.Int32Key int32Key:
                builder.Int32Key = int32Key.Key;
                break;
            case Key.Int64Key int64Key:
                builder.Int64Key = int64Key.Key;
                break;
            case Key.StringKey stringKey:
                builder.StringKey = stringKey.Key;
                break;
            case Key.NoKey _:
                builder.NoKey = true;
                break;
        }

        return builder;
    }

    public static Key ToAddressableKey(this AddressableKeyProto proto)
    {
        return (int)proto.KeyCase switch
        {
            AddressableKeyProto.Int32KeyFieldNumber => new Key.Int32Key(proto.Int32Key),
            AddressableKeyProto.Int64KeyFieldNumber => new Key.Int64Key(proto.Int64Key),
            AddressableKeyProto.StringKeyFieldNumber => new Key.StringKey(proto.StringKey),
            AddressableKeyProto.NoKeyFieldNumber => new Key.NoKey(),
            _ => throw new InvalidOperationException("Invalid key type")
        };
    }

    public static AddressableLeaseProto ToAddressableLeaseProto(this AddressableLease lease)
    {
        return new AddressableLeaseProto
        {
            NodeId = lease.NodeId.ToNodeIdProto(),
            Reference = lease.Reference.ToAddressableReferenceProto(),
            ExpiresAt = lease.ExpiresAt.ToTimestampProto(),
            RenewAt = lease.RenewAt.ToTimestampProto()
        };
    }

    public static AddressableLease ToAddressableLease(this AddressableLeaseProto proto)
    {
        return new AddressableLease
        {
            NodeId = proto.NodeId.ToNodeId(),
            Reference = proto.Reference.ToAddressableReference(),
            ExpiresAt = proto.ExpiresAt.ToTimestamp(),
            RenewAt = proto.RenewAt.ToTimestamp()
        };
    }

    public static NamespacedAddressableReferenceProto ToNamespacedAddressableReferenceProto(
        this NamespacedAddressableReference reference)
    {
        return new NamespacedAddressableReferenceProto
        {
            Namespace = reference.Namespace,
            AddressableReference = reference.AddressableReference.ToAddressableReferenceProto()
        };
    }

    public static NamespacedAddressableReference ToNamespacedAddressableReference(
        this NamespacedAddressableReferenceProto proto)
    {
        return new NamespacedAddressableReference
        {
            Namespace = proto.Namespace,
            AddressableReference = proto.AddressableReference.ToAddressableReference()
        };
    }
}