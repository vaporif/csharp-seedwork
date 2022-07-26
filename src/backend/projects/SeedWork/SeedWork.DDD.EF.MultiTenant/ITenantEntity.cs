using System;

public interface ITenantEntity
{
    Guid TenantId { get; }

    bool HasTenantId {get;}

    void SetTenantId(Guid tenantId);
}
