using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.Api.Common;
using ConferencePlanner.Api.Data;
using ConferencePlanner.Api.DataLoader;

namespace ConferencePlanner.Api.Attendees
{
    public class CheckInAttendeePayload : AttendeePayloadBase
    {
        private readonly int? _sessionId;

        public CheckInAttendeePayload(Attendee attendee, int sessionId)
            : base(attendee)
        {
            _sessionId = sessionId;
        }

        public CheckInAttendeePayload(UserError error)
            : base(new[] { error })
        {
        }

        public async Task<Session?> GetSessionAsync(
            SessionByIdDataLoader sessionById,
            CancellationToken cancellationToken)
        {
            if (_sessionId.HasValue)
            {
                return await sessionById.LoadAsync(_sessionId.Value, cancellationToken);
            }

            return null;
        }
    }
}
