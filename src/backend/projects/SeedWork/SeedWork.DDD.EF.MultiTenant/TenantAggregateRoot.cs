using System;
using SeedWork.DDD;

namespace SeedWork.DDD.EF.MultiTenant;
public class TenantAggregateRoot : AggregateRoot, ITenantEntity
{
    public Guid TenantId { get; private set; }

    public bool HasTenantId => TenantId != default;

    public void SetTenantId(Guid tenantId) => TenantId = tenantId;
}
