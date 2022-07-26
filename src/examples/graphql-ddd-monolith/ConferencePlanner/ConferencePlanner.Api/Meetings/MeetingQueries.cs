using ConferencePlanner.Application.Meetings;

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
            [Service] IMeetingsRepository repo)
            => repo.GetQueryable();

        [UseFirstOrDefault]
        [UseProjection]
        public IQueryable<Meeting> GetMeetingById(
            int id,
            [Service] IMeetingsRepository repo,
            CancellationToken cancellationToken)
            => repo.GetQueryable().Where(d => d.Id == id);
    }
}
