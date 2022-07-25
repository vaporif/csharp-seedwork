using System;

namespace SeedWork.DDD;

public class AggregateRoot : ISoftDeleteEntity
{
    private List<DomainEvent> _events = new List<DomainEvent>();

    public bool IsDeleted { get; }

    public ReadOnlyCollection<DomainEvent> DomainEvents => _events.AsReadOnly();

    public void AddEvent(DomainEvent @event)
    {
        if (DomainEvents.All(f => !f.Equals(@event)))
        {
            _events.Add(@event);
        }
    }

    public void ClearEvents() => _events.Clear();

    public void SetDeleted(bool isDeleted = true) => IsDeleted = isDeleted;
}
