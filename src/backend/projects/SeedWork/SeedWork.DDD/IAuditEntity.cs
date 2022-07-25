using System;

namespace SeedWork.DDD;

public interface IAuditEntity
{
    int CreatedByUser { get; }

    Instant CreatedDate { get; }

    int LastModifiedByUser { get; }

    Instant LastModifiedDate { get; }

    void OnAdded(Instant createdDate, int createdByUser);

    void OnUpdated(Instant lastModifiedDate, int lastModifiedByUser);
}
