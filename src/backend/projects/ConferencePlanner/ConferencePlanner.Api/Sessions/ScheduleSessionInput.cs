using System;

using HotChocolate.Types.Relay;

namespace ConferencePlanner.Api.Sessions
{
    public record ScheduleSessionInput(
        [property: ID(nameof(Session))]
        int SessionId,
        [property: ID(nameof(Track))]
        int TrackId,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}
