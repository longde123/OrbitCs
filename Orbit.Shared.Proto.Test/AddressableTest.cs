/*
 * This code is a source-to-source translation from Kotlin to C#.
 */

using NUnit.Framework;
using Orbit.Shared.Addressable;

namespace Orbit.Shared.Proto;

public class AddressableTest
{
    [Test]
    public void TestAddressableReferenceConversion()
    {
        var initialRef = new AddressableReference("test", new Key.StringKey("testId"));
        var convertedRef = initialRef.ToAddressableReferenceProto();
        var endRef = convertedRef.ToAddressableReference();
        Assert.AreEqual(initialRef, endRef);
    }

    [Test]
    public void TestNamespacedAddressableReferenceConversion()
    {
        var initialRef =
            new NamespacedAddressableReference("test", new AddressableReference("test", new Key.StringKey("testId")));
        var convertedRef = initialRef.ToNamespacedAddressableReferenceProto();
        var endRef = convertedRef.ToNamespacedAddressableReference();
        Assert.AreEqual(initialRef, endRef);
    }
}