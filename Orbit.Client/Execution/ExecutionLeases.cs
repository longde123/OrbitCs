using System.Collections.Concurrent;
using Orbit.Client.Mesh;
using Orbit.Shared.Addressable;
using Orbit.Util.Time;

namespace Orbit.Client.Execution;

public class ExecutionLeases
{
    private readonly AddressableLeaser _addressableLeaser;
    private readonly Clock _clock;
    private readonly ConcurrentDictionary<AddressableReference, AddressableLease> _currentLeases = new();

    public ExecutionLeases(AddressableLeaser addressableLeaser, Clock clock)
    {
        _addressableLeaser = addressableLeaser;
        _clock = clock;
    }

    public AddressableLease? GetLease(AddressableReference addressableReference)
    {
        if (_currentLeases.TryGetValue(addressableReference, out var addressableValue))
        {
            return addressableValue;
        }

        return null;
    }

    public async Task<AddressableLease?> GetOrRenewLease(AddressableReference addressableReference)
    {
        var currentLease = GetLease(addressableReference);

        if (currentLease == null || _clock.InPast(currentLease.ExpiresAt.ToDateTime()))
        {
            currentLease = await RenewLease(addressableReference);
        }

        return currentLease;
    }

    public async Task<AddressableLease?> RenewLease(AddressableReference addressableReference)
    {
        var newLease = await _addressableLeaser.RenewLease(addressableReference);
        if (newLease != null)
        {
            _currentLeases[addressableReference] = newLease;
        }

        return newLease;
    }

    public async Task<bool> AbandonLease(AddressableReference addressableReference)
    {
        if (_currentLeases.TryGetValue(addressableReference, out var currentLease))
        {
            return await _addressableLeaser.AbandonLease(addressableReference);
        }

        return false;
    }
}