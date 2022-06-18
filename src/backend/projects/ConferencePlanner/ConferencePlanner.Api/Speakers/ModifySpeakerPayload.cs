using ConferencePlanner.Api.Common;
using ConferencePlanner.Api.Data;

namespace ConferencePlanner.Api.Speakers
{
    public class ModifySpeakerPayload : SpeakerPayloadBase
    {
        public ModifySpeakerPayload(Speaker speaker)
            : base(speaker)
        {
        }

        public ModifySpeakerPayload(UserError error)
            : base(new [] { error })
        {
        }
    }
}
