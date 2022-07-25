using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Dynamic;
using NodaTime;
using SeedWork.DDD;

namespace SeedWork.DDD.EF;

public static class ContextExtensions
{
    public static async ValueTask<SaveOperationResult> BoundedContextSaveChangesAsync(
        this DbContext context,
        IEventDispatcher eventDispatcher,
        IClock clock,
        int userId,
        Func<Task<CancellationToken, int>> saveChangesAsync,
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

        var addedEntities = new List<IAuditEntity>();
        var updatedEntities = new List<IAuditEntity>();
        var entries = context.ChangeTracker.Entries().ToArray();

        foreach (var entry in context.ChangeTracker.Entries().ToArray())
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
                        auditEntity.OnAdded(clock.UtcNow, userId);
                        addedEntities.Add(auditEntity);
                        break;
                    case EntityState.Modified:
                        auditEntity.OnUpdated(clock.UtcNow, userId);
                        updatedEntities.Add(auditEntity);
                        break;
                    default: break;
                }
            }

            if (entry.State == EntityState.Deleted && entry.Entity is ISoftDeleteEntity softDeleteEntity)
            {
                context.Entry(entry).State = EntityState.Modified;
                entity.SetDeleted(true);
            }

            if (entity.Entity is IOnSavingEntityBehavior onSavingEntityBehavior)
            {
                onSavingEntityBehavior.OnSaving(entity.State);
            }

            if (entity.Entity is AggregateRoot aggregateRoot)
            {
                foreach (var domainEvent in aggregateRoot.DomainEvents)
                {
                    await eventDispatcher.DispatchAsync(domainEvent, ct);
                }

                aggregateRoot.ClearDomainEvents();
            }

            var rowsCount = await saveChangesAsync(ct);

            return new SaveOperationResult(rowsCount, addedEntities, updatedEntities);
        }
    }
}
