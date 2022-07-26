using ConferencePlanner.Application.Meetings;
using ConferencePlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.Infrastructure.Meetings;
public class MeetingsRepository : IMeetingsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public MeetingsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async ValueTask<Meeting> AddAsync(Meeting entity, CancellationToken ct = default)
    {
        var e = await _dbContext.AddAsync(entity, ct);
        return e.Entity;
    }

    public bool Delete(int id)
    {
        var entity = _dbContext.Meetings.Find(id);
        if (entity == null)
        {
            return false;
        }

        _dbContext.Meetings.Remove(entity);
        return true;
    }

    public async ValueTask<Meeting?> FindAsync(int id)
    {
        var entity = await _dbContext.Meetings.FindAsync(id);
        if (entity is null)
        {
            return null;
        }

        var entry = _dbContext.Entry(entity);
        await entry.Collection(t => t.Participiants).LoadAsync();
        await entry.Reference(t => t.Organizer).LoadAsync();

        return entity;
    }

    public IQueryable<Meeting> GetQueryable() => _dbContext.Meetings.AsNoTracking();

    public async ValueTask SaveChangesAsync(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}
