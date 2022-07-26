using ConferencePlanner.Application.Meetings;
using ConferencePlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.Infrastructure.Meetings;
public class MeetingsRepository : IMeetingsRepository
{
    private readonly BoundedContext<ApplicationDbContext> _context;

    public MeetingsRepository(BoundedContext<ApplicationDbContext> context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async ValueTask<Meeting> AddAsync(Meeting entity, CancellationToken ct = default)
    {
        var e = await _context.DbContext.AddAsync(entity, ct);
        return e!.Entity!;
    }

    public bool Delete(int id)
    {
        var entity = _context.DbContext.Meetings.Find(id);
        if (entity is null)
        {
            return false;
        }

        _context.DbContext.Meetings.Remove(entity);
        return true;
    }

    public async ValueTask<Meeting?> FindAsync(int id)
    {
        var entity = await _context.DbContext.Meetings.FindAsync(id);
        if (entity is null)
        {
            return null;
        }

        var entry = _context.DbContext.Entry(entity);
        await entry.Collection(t => t.Participiants).LoadAsync();
        await entry.Reference(t => t.Organizer).LoadAsync();

        return entity;
    }

    public IQueryable<Meeting> GetQueryable() => _context.DbContext.Meetings.AsNoTracking();

    public async ValueTask SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}
