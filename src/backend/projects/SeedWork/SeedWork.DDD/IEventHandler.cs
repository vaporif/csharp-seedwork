using System;

namespace SeedWork.DDD
{
    public interface IEventHandler<TEvent>
        where TEvent : IDomainEvent
    {
        ValueTask HandleAsync(TEvent @event, CancellationToken ct = default);
    }
}
