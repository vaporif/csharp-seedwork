using System;
using SeedWork.DDD;

public class TenantAggregateRoot : AggregateRoot, ITenantEntity
{
    public Guid TenantId { get; private set; }

    public bool HasTenantId => TenantId != default;

    public void SetTenantId(Guid tenantId) => TenantId = tenantId;
}
