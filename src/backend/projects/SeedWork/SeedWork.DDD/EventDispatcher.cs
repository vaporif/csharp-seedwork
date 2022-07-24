using System;

namespace SeedWork.DDD
{
    // NOTE: This class is used by the framework to dispatch events. Is a service locator.
    public sealed class EventDispatcher : IEventDispatcher
    {
        public ValueTask DispatchAsync(DomainEvent @event, CancellationToken ct = default)
        {

        }
    }
}
