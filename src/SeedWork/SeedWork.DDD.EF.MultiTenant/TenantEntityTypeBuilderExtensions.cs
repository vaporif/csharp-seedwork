
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class TenantEntityTypeBuilderExtensions
{
    public static void HasTenantQueryFilter<T>(this EntityTypeBuilder<T> entity, Guid tenantId)
        where T : class, ITenantEntity
    {
        entity.HasQueryFilter(x => x.TenantId == tenantId);
    }
}
