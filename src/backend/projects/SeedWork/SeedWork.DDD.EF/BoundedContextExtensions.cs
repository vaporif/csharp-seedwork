using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Dynamic;

namespace SeedWork.DDD.EF;

public static class ContextExtensions
{
    public static void TryAttach(this DbContext dbContext, object entity)
    {
        if (dbContext.Entry(entity).State == EntityState.Detached)
        {
            dbContext.Attach(entity);
        }
    }

    public static object GetPrimaryKeyObject(this DbContext dbContext, object entity)
    {
        var keyProps = dbContext.GetPrimaryKeyProperties(entity).ToList();
        var expandoKeyObject = new ExpandoObject();
        var expandoCollection = (ICollection<KeyValuePair<string, object>>)expandoKeyObject!;
        keyProps.ForEach(x =>
            expandoCollection.Add(new KeyValuePair<string, object>(x.Name, entity.GetPropertyValue(x.Name))));
        return expandoKeyObject;
    }

    public static IList<object> GetPrimaryKeyValues(this DbContext dbContext, object entity)
    {
        var keyProps = dbContext.GetPrimaryKeyProperties(entity) ?? new List<IProperty>();
        return keyProps.Select(x => entity.GetPropertyValue(x.Name)).ToList();
    }

    public static IList<IProperty> GetPrimaryKeyProperties(this DbContext dbContext, object entity)
    {
        return dbContext.Model.FindEntityType(entity.GetType())?.FindPrimaryKey()?.Properties.ToList()!;
    }

    public static void SetTenantIdForEntities(this DbContext context, int tenantId)
    {
        SetForEntities();
        SetForNullableEntities();

        // Tenant Entities
        void SetForEntities()
        {
            var entities = context.ChangeTracker.Entries().Where(x => x.State is EntityState.Added or EntityState.Modified)
                .Select(x => x.Entity).OfType<ITenantEntity>().ToList();

            entities.ForEach(x =>
            {
                if (x.TenantId == 0)
                {
                    x.SetTenantId(tenantId);
                }
            });
        }

        // Nullable Tenant Entities
        void SetForNullableEntities()
        {
            var entities = context.ChangeTracker.Entries().Where(x => x.State is EntityState.Added or EntityState.Modified)
                .Select(x => x.Entity).OfType<INullableTenantEntity>().ToList();

            entities.ForEach(x =>
            {
                if (!x.TenantId.HasValue || x.TenantId == 0)
                {
                    x.SetTenantId(tenantId);
                }
            });
        }
    }

    public static void SetIsDeletedForDeletedEntities(this DbContext context)
    {
        var entities = context.ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted)
            .Select(x => x.Entity).OfType<ISoftDeleteEntity>();

        foreach (var entity in entities)
        {
            context.Entry(entity).State = EntityState.Modified;
            entity.SetDeleted(true);
        }
    }

    public static void SetAuditEntityDetails(this DbContext context, IClock clock, int userId)
    {
        var added = context.ChangeTracker.Entries().Where(x => x.State == EntityState.Added)
            .Select(x => x.Entity).OfType<IAuditEntity>();

        var modified = context.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified)
            .Select(x => x.Entity).OfType<IAuditEntity>();

        foreach (var entity in added)
        {
            entity.OnAdded(clock.UtcNow, userId);
        }

        foreach (var entity in modified)
        {
            entity.OnUpdated(clock.UtcNow, userId);
        }
    }

    public static void InvokeOnSavingEntityBehaviors(this DbContext context)
    {
        GetEntities(EntityState.Added).ToList().ForEach(x => x.OnSaving(SavingEntityState.Added));
        GetEntities(EntityState.Modified).ToList().ForEach(x => x.OnSaving(SavingEntityState.Updated));
        GetEntities(EntityState.Deleted).ToList().ForEach(x => x.OnSaving(SavingEntityState.Deleted));

        IList<IOnSavingEntityBehavior> GetEntities(EntityState state) => context.ChangeTracker.Entries()
            .Where(x => x.State == state).Select(x => x.Entity).OfType<IOnSavingEntityBehavior>().ToList();
    }

    public static void AddDomainEventsToEntities(this DbContext context)
    {
        var addedEntities = GetNonSuppressedDomainEntities(EntityState.Added).ToList();
        var deletedEntities = GetNonSuppressedDomainEntities(EntityState.Deleted).ToList();
        var updatedEntities = GetNonSuppressedDomainEntities(EntityState.Modified).ToList();

        addedEntities.ForEach(entity =>
        {
            var getId = () =>
            {
                var entityIdList = context.GetPrimaryKeyValues(entity);
                return string.Join(",", entityIdList.Select(x => x?.ToString()));
            };
            entity.DomainEvents.Add(new EntityAddedEvent(entity, getId));
        });

        deletedEntities.ForEach(entity =>
        {
            var entityIdList = context.GetPrimaryKeyValues(entity);
            var entityIdString = string.Join(",", entityIdList.Select(x => x?.ToString()));
            entity.DomainEvents.Add(new EntityDeletedEvent(entity, entityIdString));
        });

        updatedEntities.ForEach(entity =>
        {
            var entityIdList = context.GetPrimaryKeyValues(entity);
            var entityIdString = string.Join(",", entityIdList.Select(x => x?.ToString()));
            var changes = context.Entry(entity).GetPropertyChanges();
            if (!changes.Any())
            {
                return;
            }

            entity.DomainEvents.Add(new EntityUpdatedEvent(entity, entityIdString, changes));
            if (entity is IOnPropertiesChangedEntityBehavior behavior)
            {
                behavior.OnPropertiesChanged(changes);
            }
        });

        IList<IDomainEntity> GetNonSuppressedDomainEntities(EntityState state)
            => context.GetDomainEntities(state).Where(x => x is not ISuppressDomainEventsBehavior).ToList();
    }

    public static IList<IDomainEvent> GetValueObjectDomainEvents(this DbContext context)
    {
        var ownedEntries = context.ChangeTracker.Entries().Where(x => x.Metadata.IsOwned()).ToList();
        var added = ownedEntries.Where(x => x.State == EntityState.Added);
        var deleted = ownedEntries.Where(x => x.State == EntityState.Deleted);
        var joined = added.Join(deleted,
            x => new { Name = x.Metadata.FindOwnership()?.PrincipalToDependent?.Name, ForeignKeys = x.Metadata.GetForeignKeys() },
            y => new { Name = y.Metadata.FindOwnership()?.PrincipalToDependent?.Name, ForeignKeys = y.Metadata.GetForeignKeys() },
            (x, y) => new { Added = x, Deleted = y });

        var domainEvents = new List<IDomainEvent>();
        foreach (var e in joined)
        {
            var ownership = e.Deleted.Metadata.FindOwnership()!;
            var principalName = ownership.PrincipalEntityType.DisplayName();
            var principalProperty = ownership.PrincipalToDependent?.Name;
            var foreignKeyNames = ownership.Properties.Select(x => x.Name);
            var foreignKeyValues = foreignKeyNames.Select(x => e.Deleted.Property(x).CurrentValue);
            var name = $"{principalName}.{principalProperty}:{string.Join(",", foreignKeyNames)}";

            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            var currentValues = e.Added.CurrentValues;
            e.Added.State = EntityState.Detached;
            e.Deleted.State = EntityState.Modified;
            e.Deleted.CurrentValues.SetValues(currentValues);
            var changes = e.Deleted.GetPropertyChanges();
            if (changes.Any())
            {
                var entityIdString = string.Join(",", foreignKeyValues);
                domainEvents.Add(new EntityUpdatedEvent(name, entityIdString, changes));

                if (e.Deleted.Entity is IOnPropertiesChangedEntityBehavior behavior)
                {
                    behavior.OnPropertiesChanged(changes);
                }
            }
        }

        return domainEvents.OfType<IDomainEvent>().ToList();
    }

    public static IList<IDomainEntity> GetDomainEntities(this DbContext context, EntityState? state = default!)
        => context.ChangeTracker.Entries()
            .Where(x => !state.HasValue || x.State == state.Value)
            .Select(x => x.Entity)
            .OfType<IDomainEntity>()
            .ToList();

    public static IList<IDomainEvent> GetDomainEvents(this DbContext context, EntityState? state = default!)
    {
        var entityEvents = context.GetDomainEntities(state)
          .SelectMany(x => x.DomainEvents)
          .OfType<IDomainEvent>()
          .ToList();

        if (context is CrowdPass.MinApi.Modules.CrowdPassContext crowdPassContext)
        {
            entityEvents.AddRange(crowdPassContext.ValueObjectDomainEvents);
        }

        return entityEvents;
    }

    public static bool HasDomainEvents(this DbContext context, EntityState? state = default!)
        => context.GetDomainEvents(state).Any();

    public static void ClearDomainEvents(this DbContext context, EntityState? state = default!)
    {
        var domainEntities = context.GetDomainEntities(state);
        foreach (var domainEntity in domainEntities)
        {
            domainEntity.DomainEvents.Clear();
        }

        if (context is CrowdPass.MinApi.Modules.CrowdPassContext crowdPassContext)
        {
            crowdPassContext.ValueObjectDomainEvents.Clear();
        }
    }

    public static async Task<int> HandleDomainEventsAsync(this DbContext context, IEventRunner runner, Func<Task<int>> saveChanges)
    {
        var result = await saveChanges();
        var domainEvents = context.GetDomainEvents();
        context.ClearDomainEvents();
        foreach (var domainEvent in domainEvents)
        {
            await runner.PublishDomainEvent(domainEvent);
        }
        
        result += await saveChanges();

        return result;
    }

    public static IList<UpdatedEntityPropertyChange> GetPropertyChanges(this EntityEntry entry)
    {
        var changeList = new List<UpdatedEntityPropertyChange>();

        entry.Properties.ToList().ForEach(prop =>
        {
            if (!prop.IsModified) return;

            var propName = prop.Metadata.Name;
            var newValue = prop.CurrentValue;
            var oldValue = prop.OriginalValue;
            if (!Object.Equals(oldValue, newValue))
            {
                changeList.Add(new UpdatedEntityPropertyChange(propName, newValue!, oldValue!));
            }
        });

        return changeList;
    }

    public static UpdatedEntityPropertyChange? TryGetChange(
        this IList<UpdatedEntityPropertyChange> changes, string propertyName)
            => changes.FirstOrDefault(x => x.PropertyName.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
}
