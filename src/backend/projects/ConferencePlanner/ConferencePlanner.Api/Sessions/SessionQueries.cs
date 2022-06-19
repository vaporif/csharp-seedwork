using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.Api.Data;
using ConferencePlanner.Api.DataLoader;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.Api.Sessions
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class SessionQueries
    {
        // 
        [UseOffsetPaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering(typeof(SessionFilterInputType))]
        [UseSorting]
        public IQueryable<Session> GetSessions(
            ApplicationDbContext context)
            => context.Sessions.OrderBy(d => d.Id);

        public Task<Session> GetSessionByIdAsync(
            [ID(nameof(Session))] int id,
            SessionByIdDataLoader sessionById,
            CancellationToken cancellationToken)
            => sessionById.LoadAsync(id, cancellationToken);

        public async Task<IEnumerable<Session>> GetSessionsByIdAsync(
            [ID(nameof(Session))] int[] ids,
            SessionByIdDataLoader sessionById,
            CancellationToken cancellationToken)
            => await sessionById.LoadAsync(ids, cancellationToken);
    }
}
