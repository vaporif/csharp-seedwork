using System;

public class DeletableTenantEntity : SoftDeleteEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }

    public bool HasTenantId => TenantId != default;

    public void SetTenantId(Guid tenantId) => TenantId = tenantId; 
}
