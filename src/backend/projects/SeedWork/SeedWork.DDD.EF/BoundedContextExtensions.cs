global using System;
global using System.Linq;
global using Microsoft.EntityFrameworkCore;

public static class BoundedContextExtensions
{
    public static async ValueTask<SaveOperationResult> BoundedContextSaveChangesAsync(
        this DbContext context,
        MediatR.IPublisher eventDispatcher,
        IClock clock,
        int userId,
        Func<CancellationToken, ValueTask<int>> saveChangesAsync,
        CancellationToken ct = default)
    {
        if (eventDispatcher is null)
        {
            throw new ArgumentNullException(nameof(eventDispatcher));
        }

        if (clock is null)
        {
            throw new ArgumentNullException(nameof(clock));
        }

        var addedEntities = new HashSet<IAuditEntity>();
        var updatedEntities = new HashSet<IAuditEntity>();

        var rowsCount = 0;

        var entries = context.ChangeTracker.Entries().ToArray();

        do
        {
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                {
                    continue;
                }

                if (entry.Entity is IAuditEntity auditEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            if (!addedEntities.Contains(auditEntity))
                            {
                                auditEntity.OnAdded(clock.UtcNow, userId);
                                addedEntities.Add(auditEntity);
                            }
                            break;
                        case EntityState.Modified:
                            if (!updatedEntities.Contains(auditEntity))
                            {
                                auditEntity.OnUpdated(clock.UtcNow, userId);
                                updatedEntities.Add(auditEntity);
                            }

                            break;
                        default: break;
                    }
                }

                if (entry.State == EntityState.Deleted && entry.Entity is ISoftDeleteEntity softDeleteEntity)
                {
                    context.Entry(entry).State = EntityState.Modified;
                    softDeleteEntity.SetDeleted(true);
                }

                if (entry.Entity is AggregateRoot aggregateRoot)
                {
                    foreach (var domainEvent in aggregateRoot.DomainEvents)
                    {
                        await eventDispatcher.Publish(domainEvent, ct);
                    }

                    aggregateRoot.ClearDomainEvents();
                }

                rowsCount += await saveChangesAsync(ct);

                entries = context
                    .ChangeTracker
                    .Entries()
                    .Where(f => 
                        (f.Entity is AggregateRoot a && a.DomainEvents.Any()) || 
                        (entry.State != EntityState.Detached && entry.State == EntityState.Unchanged))
                    .ToArray();
            }
        } while (entries.Any());

        return new SaveOperationResult(rowsCount, addedEntities.ToList(), updatedEntities.ToList());
    }
}
