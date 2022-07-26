using Microsoft.EntityFrameworkCore.Metadata.Builders;

public abstract class IntegrationEventLogDbContext : DbContext
{
    public IntegrationEventLogDbContext(DbContextOptions options) : base(options) 
    {
    }

    public DbSet<IntegrationEventLogEntry> IntegrationEventLogs { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(GetSchemaName());
        modelBuilder.Entity<IntegrationEventLogEntry>(ConfigureIntegrationEventLogEntry);
    }

    protected abstract string GetSchemaName();

    protected virtual void ConfigureIntegrationEventLogEntry(EntityTypeBuilder<IntegrationEventLogEntry> builder)
    {
        builder.ToTable("IntegrationEventLogs");

        builder.HasKey(e => e.EventId);

        builder.Property(e => e.EventId)
            .IsRequired();

        builder.Property(e => e.Content)
            .IsRequired();

        builder.Property(e => e.CreationTime)
            .IsRequired();

        builder.Property(e => e.State)
            .IsRequired();

        builder.Property(e => e.TimesSent)
            .IsRequired();

        builder.Property(e => e.EventTypeName)
            .IsRequired();
    }
}
