using ConferencePlanner.Api.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.Api.Tracks
{
    public record RenameTrackInput(
        [property: ID(nameof(Track))] int Id, 
        string Name);
}
