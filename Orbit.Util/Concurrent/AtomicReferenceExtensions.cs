namespace Orbit.Util.Concurrent;

public static class AtomicReferenceExtensions
{
    public static T AtomicSet<T>(this AtomicReference<T> atomicReference, Func<T, T> block)
    {
        var atomicReferenceCompareAndSet = false;
        do
        {
            var initialValue = atomicReference.Get();
            var newValue = block(initialValue);
            atomicReferenceCompareAndSet = atomicReference.CompareAndSet(initialValue, newValue);
        } while (!atomicReferenceCompareAndSet);

        return atomicReference.Get();
    }
}