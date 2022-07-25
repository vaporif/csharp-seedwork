using System;

namespace SeedWork.DDD.EF.MultiTenant;
public interface ITenantEntity
{
    Guid TenantId { get; }

    bool HasTenantId {get;}

    void SetTenantId(Guid tenantId);
}
