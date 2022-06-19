using ConferencePlanner.Api.Common;


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
