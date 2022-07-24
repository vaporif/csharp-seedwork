namespace SeedWork.Multiservice
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public abstract class IntegrationEventLogDbContext : DbContext
    {
        public IntegrationEventLogDbContext(DbContextOptions options) : base(options) => options = options ?? throw new ArgumentNullException(nameof(options));

        public DbSet<IntegrationEventLogEntry> IntegrationEventLogs { get; set; }

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
}
