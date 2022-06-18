using ConferencePlanner.Api.Data;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.Api.Speakers
{
    public record UploadSpeakerPhotoInput([ID(nameof(Speaker))]int Id, IFile Photo);
}
