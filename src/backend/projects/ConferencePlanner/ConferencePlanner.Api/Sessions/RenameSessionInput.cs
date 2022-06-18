using ConferencePlanner.Api.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.Api.Sessions
{
    public record RenameSessionInput(
        [property: ID(nameof(Session))] string SessionId,
        string Title);
}
