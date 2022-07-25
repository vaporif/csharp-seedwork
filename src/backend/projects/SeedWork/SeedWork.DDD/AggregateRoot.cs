using System.Collections.ObjectModel;

public class AggregateRoot : ISoftDeleteEntity, IAuditEntity
{
    private List<DomainEvent> _events = new List<DomainEvent>();

    public bool IsDeleted { get; private set; }

    public int CreatedByUser { get; private set; }

    public Instant CreatedDate { get; private set; }

    public int LastModifiedByUser { get; private set; }

    public Instant LastModifiedDate { get; private set; }

    public ReadOnlyCollection<DomainEvent> DomainEvents => _events.AsReadOnly();

    public void AddEvent(DomainEvent @event)
    {
        if (DomainEvents.All(f => !f.Equals(@event)))
        {
            _events.Add(@event);
        }
    }

    public void ClearDomainEvents()
    {
        if(_events.Any())
        {
            _events.Clear();
        }
    } 

    public void SetDeleted(bool isDeleted = true) => IsDeleted = isDeleted;

    public void OnAdded(Instant createdDate, int createdByUser)
    {
        CreatedDate = createdDate;
        CreatedByUser = createdByUser;
    }

    public void OnUpdated(Instant lastModifiedDate, int lastModifiedByUser)
    {
        LastModifiedByUser = lastModifiedByUser;
        LastModifiedDate = lastModifiedDate;
    }
}
