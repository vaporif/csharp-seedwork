using System;

namespace SeedWork.DDD
{
    public interface IEventDispatcher
    {
        ValueTask DispatchAsync(DomainEvent @event, CancellationToken ct = default);
    }
}
