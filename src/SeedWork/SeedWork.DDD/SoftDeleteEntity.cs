public class SoftDeleteEntity : ISoftDeleteEntity, IAuditEntity
{
    public int CreatedByUser { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public int LastModifiedByUser { get; set; }

    public DateTimeOffset LastModifiedDate { get; set; }

    public bool IsDeleted { get; set; }

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
