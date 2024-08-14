using Orbit.Shared.Net;
using AddressableType = System.String;
using AddressableInvocationArgument = System.Tuple<object, System.Type>;
using AddressableInvocationArguments = System.Collections.Generic.List<System.Tuple<object, System.Type>>;

namespace Orbit.Shared.Addressable;

public class AddressableReference : IEquatable<AddressableReference>
{
    public AddressableReference()
    {
    }

    public AddressableReference(AddressableType t, Key k)
    {
        Type = t;
        Key = k;
    }

    public AddressableType Type { get; set; }
    public Key Key { get; set; }

    public bool Equals(AddressableReference? obj)
    {
        return obj.Type.Equals(Type) && obj.Key.Equals(Key);
    }

    public override bool Equals(object? obj)
    {
        if (obj != null && obj is AddressableReference nid)
        {
            return Equals(nid);
        }

        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            // From http://stackoverflow.com/a/263416/613130
            var hash = 17;
            hash = hash * 23 + Type.GetHashCode();
            hash = hash * 23 + Key.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        return Key.ToString();
    }
}

public class NamespacedAddressableReference : IEquatable<NamespacedAddressableReference>
{
    public NamespacedAddressableReference()
    {
    }

    public NamespacedAddressableReference(string ns, AddressableReference addressableReference)
    {
        Namespace = ns;
        AddressableReference = addressableReference;
    }

    public string Namespace { get; set; }
    public AddressableReference AddressableReference { get; set; }

    public bool Equals(NamespacedAddressableReference? obj)
    {
        return obj.AddressableReference.Equals(AddressableReference) && obj.Namespace.Equals(Namespace);
    }

    public override bool Equals(object? obj)
    {
        if (obj != null && obj is NamespacedAddressableReference nid)
        {
            return Equals(nid);
        }

        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            // From http://stackoverflow.com/a/263416/613130
            var hash = 17;
            hash = hash * 23 + Namespace.GetHashCode();
            hash = hash * 23 + AddressableReference.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        return Namespace + AddressableReference;
    }
}

public class AddressableInvocation
{
    public AddressableReference Reference { get; set; }
    public string Method { get; set; }
    public AddressableInvocationArguments Args { get; set; }
    public InvocationReason Reason { get; set; } = InvocationReason.Invocation;

    public override bool Equals(object obj)
    {
        if (this == obj)
        {
            return true;
        }

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (AddressableInvocation)obj;

        if (!Reference.Equals(other.Reference))
        {
            return false;
        }

        if (!Method.Equals(other.Method))
        {
            return false;
        }

        if (!Args.SequenceEqual(other.Args))
        {
            return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        var result = Reference.GetHashCode();
        result = 31 * result + Method.GetHashCode();
        result = 31 * result + Args.GetHashCode();
        return result;
    }
}