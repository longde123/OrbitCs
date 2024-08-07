using Orbit.Client.Actor;
using Orbit.Client.Addressable;

namespace Orbit.Client.Stream;
[NonConcrete]
public interface IActorStream<T> : IAsyncSubject<T>, IActorWithStringKey
{
    
}