using System;

public interface INullableTenantEntity
{
    Guid? TenantId { get; }

    bool HasTenantId { get; }

    void SetTenantId(Guid? tenantId);
}
