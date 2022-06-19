
using HotChocolate;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.Api.Speakers
{
    public record ModifySpeakerInput(
        [property: ID(nameof(Speaker))] 
        int Id,
        Optional<string?> Name,
        Optional<string?> Bio,
        Optional<string?> WebSite);
}
