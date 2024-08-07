using Orbit.Shared.Addressable;

namespace Orbit.Client.Addressable;

using AddressableClass = Type;

/**
 * Denotes an addressable that does not have a concrete implementation.
 */
[AttributeUsage(AttributeTargets.Interface)]
public class NonConcrete : Attribute
{
}

/**
 * Marker interface that determines an interface is addressable remotely.
 */
[NonConcrete]
public interface IAddressable
{
}

/**
 * A class type which extends Addressable.
 * Denotes a method which is executed on activation for lifecycle managed Addressables.
 */
[AttributeUsage(AttributeTargets.Method)]
public class OnActivate : Attribute
{
}

/**
 * Denotes a method which is executed on deactivation for lifecycle managed Addressables.
 */
[AttributeUsage(AttributeTargets.Method)]
public class OnDeactivate : Attribute
{
}

/**
 * Denotes the reason an addressable is deactivating.
 */
public enum DeactivationReason
{
    TtlExpired,
    NodeShuttingDown,
    LeaseRenewalFailed,
    ExternallyTriggered
}

/**
 * An abstract Addressable which allows Orbit to provide an AddressableContext.
 */
public abstract class AbstractAddressable
{
 /**
     * The Orbit context. It will be available after the Addressable is registered with Orbit.
     * Attempting to access this variable before registration is undefined behavior.
     */
 public AddressableContext Context { get; set; }
}

/**
 * A context available to an Addressable which gives access to Orbit runtime information.
 */
public class AddressableContext
{
    public AddressableContext(AddressableReference reference, OrbitClient client)
    {
        Reference = reference;
        Client = client;
    }

    /**
     * A reference to this Addressable.
     */
    public AddressableReference Reference { get; }

    /**
     * A reference to the OrbitClient.
     */
    public OrbitClient Client { get; }
}