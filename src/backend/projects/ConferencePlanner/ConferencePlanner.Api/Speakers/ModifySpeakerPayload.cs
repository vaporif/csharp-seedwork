using ConferencePlanner.Api.Common;


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
