public interface IAuditEntity
{
    int CreatedByUser { get; }

    DateTimeOffset CreatedDate { get; }

    int LastModifiedByUser { get; }

    DateTimeOffset LastModifiedDate { get; }

    void OnAdded(DateTimeOffset createdDate, int createdByUser);

    void OnUpdated(DateTimeOffset lastModifiedDate, int lastModifiedByUser);
}
