using System;

namespace SeedWork.DDD.EF.MultiTenant;
public interface INullableTenantEntity
{
    Guid? TenantId { get; }

    bool HasTenantId { get; }

    void SetTenantId(Guid? tenantId);
}
