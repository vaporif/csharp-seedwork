using System;
using Microsoft.EntityFrameworkCore;

public static class TenantBoundedContextExtensions
{
    public static void SetTenantIdForEntities(this DbContext context, Guid tenantId)
    {
        var entities = context.ChangeTracker
            .Entries()
            .Where(x => x.State is EntityState.Added or EntityState.Modified)
            .ToArray();

        foreach (var entity in entities)
        {
            if (entity is ITenantEntity tenantEntity && !tenantEntity.HasTenantId)
            {
                tenantEntity.SetTenantId(tenantId);
            }

            if (entity is INullableTenantEntity nullTenantEntity && !nullTenantEntity.HasTenantId)
            {
                nullTenantEntity.SetTenantId(tenantId);
            }
        }
    }
}
