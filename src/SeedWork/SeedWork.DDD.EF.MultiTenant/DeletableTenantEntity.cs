using System;

public abstract class DeletableTenantEntity : SoftDeleteEntity, ITenantEntity
{
    public Guid TenantId { get; set; }

    public bool HasTenantId => TenantId != default;

    public void SetTenantId(Guid tenantId) => TenantId = tenantId;
}
