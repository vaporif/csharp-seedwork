namespace SeedWork.DDD;

public interface IDomainEventHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{
    ValueTask HandleAsync(TDomainEvent @event, CancellationToken ct = default);
}
