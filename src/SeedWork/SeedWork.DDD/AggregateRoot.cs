using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using HotChocolate;

public class AggregateRoot : ISoftDeleteEntity, IAuditEntity
{
    [NotMapped]
    [GraphQLIgnore]
    private List<DomainEvent> _events = new List<DomainEvent>();

    public bool IsDeleted { get; set; }

    public int CreatedByUser { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public int LastModifiedByUser { get; set; }

    public DateTimeOffset LastModifiedDate { get; set; }

    [NotMapped]
    [GraphQLIgnore]
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
        if (_events.Any())
        {
            _events.Clear();
        }
    }

    public virtual void SetDeleted(bool isDeleted = true) => IsDeleted = isDeleted;

    public virtual void OnAdded(DateTimeOffset createdDate, int createdByUser)
    {
        CreatedDate = createdDate;
        CreatedByUser = createdByUser;
    }

    public virtual void OnUpdated(DateTimeOffset lastModifiedDate, int lastModifiedByUser)
    {
        LastModifiedByUser = lastModifiedByUser;
        LastModifiedDate = lastModifiedDate;
    }
}
