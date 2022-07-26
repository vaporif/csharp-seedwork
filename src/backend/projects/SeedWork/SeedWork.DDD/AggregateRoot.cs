using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

public class AggregateRoot : ISoftDeleteEntity, IAuditEntity
{
    [NotMapped]
    private List<DomainEvent> _events = new List<DomainEvent>();

    public bool IsDeleted { get; private set; }

    public int CreatedByUser { get; private set; }

    public Instant CreatedDate { get; private set; }

    public int LastModifiedByUser { get; private set; }

    public Instant LastModifiedDate { get; private set; }

    [NotMapped]
    public ReadOnlyCollection<DomainEvent> DomainEvents => _events.AsReadOnly();

    public void AddDomainEvent(DomainEvent @event)
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

    public virtual void SetDeleted(bool isDeleted = true) => IsDeleted = isDeleted;

    public virtual void OnAdded(Instant createdDate, int createdByUser)
    {
        CreatedDate = createdDate;
        CreatedByUser = createdByUser;
    }

    public virtual void OnUpdated(Instant lastModifiedDate, int lastModifiedByUser)
    {
        LastModifiedByUser = lastModifiedByUser;
        LastModifiedDate = lastModifiedDate;
    }
}
