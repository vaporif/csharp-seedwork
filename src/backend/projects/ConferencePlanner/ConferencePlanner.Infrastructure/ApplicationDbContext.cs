using ConferencePlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        private readonly MediatR.IPublisher _domainEventDispatcher;
        private readonly IClock _clock;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            MediatR.IPublisher domainEventDispatcher,
            IClock clock) : base(options)
        {
            _domainEventDispatcher = domainEventDispatcher ?? throw new ArgumentNullException(nameof(domainEventDispatcher));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Attendee>()
                .HasIndex(a => a.UserName)
                .IsUnique();

            // Many-to-many: Session <-> Attendee
            modelBuilder
                .Entity<SessionAttendee>()
                .HasKey(ca => new { ca.SessionId, ca.AttendeeId });

            // Many-to-many: Speaker <-> Session
            modelBuilder
                .Entity<SessionSpeaker>()
                .HasKey(ss => new { ss.SessionId, ss.SpeakerId });
        }

        public DbSet<Session> Sessions { get; set; } = default!;

        public DbSet<Meeting> Meetings { get; set; } = default!;

        public DbSet<Track> Tracks { get; set; } = default!;

        public DbSet<Speaker> Speakers { get; set; } = default!;

        public DbSet<Attendee> Attendees { get; set; } = default!;

        // public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        // {
        //     var result = await this.BoundedContextSaveChangesAsync(
        //         _domainEventDispatcher, 
        //         _clock,
        //         0, 
        //         async (ct) => await base.SaveChangesAsync(ct), 
        //         cancellationToken);

        //     return result.AffectedRows;
        // }
    }
}
