using System;

namespace SeedWork.DDD
{
    public interface IEventHandler<TEvent>
        where TEvent : DomainEvent
    {
        ValueTask HandleAsync(TEvent @event, CancellationToken ct = default);
    }
}
