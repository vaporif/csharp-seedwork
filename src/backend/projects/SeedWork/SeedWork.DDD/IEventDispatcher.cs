public interface IDomainEventDispatcher
{
    ValueTask DispatchAsync(DomainEvent @event, CancellationToken ct = default);
}
