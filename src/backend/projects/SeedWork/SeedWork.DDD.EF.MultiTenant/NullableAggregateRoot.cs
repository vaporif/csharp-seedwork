using System;
using SeedWork.DDD;

public class NullableTenantAggregateRoot : AggregateRoot, INullableTenantEntity
{
    public Guid? TenantId { get; private set; }

    public bool HasTenantId => TenantId is not null;

    public void SetTenantId(Guid? tenantId) => TenantId = tenantId;
}
