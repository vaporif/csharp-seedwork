using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.Api.Data;
using ConferencePlanner.Api.DataLoader;
using HotChocolate;
using HotChocolate.Types;

namespace ConferencePlanner.Api.Sessions
{
    [ExtendObjectType(OperationTypeNames.Subscription)]
    public class SessionSubscriptions
    {
        [Subscribe]
        [Topic]
        public Task<Session> OnSessionScheduledAsync(
            [EventMessage] int sessionId,
            SessionByIdDataLoader sessionById,
            CancellationToken cancellationToken) =>
            sessionById.LoadAsync(sessionId, cancellationToken);
    }
}
