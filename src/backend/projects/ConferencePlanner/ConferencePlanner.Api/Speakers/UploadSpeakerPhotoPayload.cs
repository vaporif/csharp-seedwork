using ConferencePlanner.Api.Common;
using ConferencePlanner.Api.Data;

namespace ConferencePlanner.Api.Speakers
{
    public class UploadSpeakerPhotoPayload : SpeakerPayloadBase
    {
        public UploadSpeakerPhotoPayload(Speaker speaker) 
            : base(speaker)
        {
        }

        public UploadSpeakerPhotoPayload(UserError error) 
            : base(new[] { error })
        {
        }

        
    }
}
