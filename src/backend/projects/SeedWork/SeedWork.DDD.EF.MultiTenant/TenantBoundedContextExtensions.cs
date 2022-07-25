using System;
using Microsoft.EntityFrameworkCore;

public static class ContextExtensions
{
    public static void SetTenantIdForEntities(this DbContext context, Guid tenantId)
    {
        SetForEntities();
        SetForNullableEntities();

        void SetForEntities()
        {
            var entities = context.ChangeTracker.Entries().Where(x => x.State is EntityState.Added or EntityState.Modified)
                .Select(x => x.Entity).OfType<ITenantEntity>().ToArray();

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                if (entity.HasTenantId)
                {
                    entity.SetTenantId(tenantId);
                }
            }
        }

        void SetForNullableEntities()
        {
            var entities = context.ChangeTracker.Entries().Where(x => x.State is EntityState.Added or EntityState.Modified)
                .Select(x => x.Entity).OfType<INullableTenantEntity>().ToArray();

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                if (entity.HasTenantId)
                {
                    entity.SetTenantId(tenantId);
                }
            }
        }
    }
}
