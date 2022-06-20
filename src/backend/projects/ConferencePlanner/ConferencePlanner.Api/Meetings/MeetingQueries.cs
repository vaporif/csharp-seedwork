using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.Api.Meetings
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class MeetingQueries
    {
        [UseOffsetPaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Meeting> GetMeeting(
            ApplicationDbContext context)
            => context.Meetings.AsNoTracking();

        [UseFirstOrDefault]
        [UseProjection]
        public IQueryable<Meeting> GetMeetingById(
            int id,
             ApplicationDbContext context,
            CancellationToken cancellationToken)
            => context.Meetings.Where(d => d.Id == id).AsNoTracking();
    }
}
