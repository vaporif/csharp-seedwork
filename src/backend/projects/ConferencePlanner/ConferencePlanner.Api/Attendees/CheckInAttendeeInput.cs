using ConferencePlanner.Api.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.Api.Attendees
{
    public record CheckInAttendeeInput(
        [property: ID(nameof(Session))]
        int SessionId,
        [property: ID(nameof(Attendee))]
        int AttendeeId);
}
