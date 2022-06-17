namespace api.Data
{
    using api.Data.Entities;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Division>()
                .HasIndex(a => a.Name)
                .IsUnique();

            // Many-to-many: Session <-> Attendee
            modelBuilder
                .Entity<DivisionEmployee>()
                .HasKey(ca => new { ca.DivisionId, ca.EmployeeId });
        }


        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Post> Posts { get; set; } = default!;
        public DbSet<Division> Divisions { get; set; } = default!;
        public DbSet<Employee> Employees { get; set; } = default!;
    }
}
