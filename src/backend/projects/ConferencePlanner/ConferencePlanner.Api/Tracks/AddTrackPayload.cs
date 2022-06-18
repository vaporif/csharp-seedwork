using System.Collections.Generic;
using ConferencePlanner.Api.Common;
using ConferencePlanner.Api.Data;

namespace ConferencePlanner.Api.Tracks
{
    public class AddTrackPayload : TrackPayloadBase
    {
        public AddTrackPayload(Track track) 
            : base(track)
        {
        }

        public AddTrackPayload(IReadOnlyList<UserError> errors) 
            : base(errors)
        {
        }
    }
}
