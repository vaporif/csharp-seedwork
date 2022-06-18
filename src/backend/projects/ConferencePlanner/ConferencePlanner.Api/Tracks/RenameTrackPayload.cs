using System.Collections.Generic;
using ConferencePlanner.Api.Common;
using ConferencePlanner.Api.Data;

namespace ConferencePlanner.Api.Tracks
{
    public class RenameTrackPayload : TrackPayloadBase
    {
        public RenameTrackPayload(Track track) 
            : base(track)
        {
        }

        public RenameTrackPayload(IReadOnlyList<UserError> errors) 
            : base(errors)
        {
        }
    }
}
