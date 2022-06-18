using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConferencePlanner.Api.Data;
using ConferencePlanner.Api.DataLoader;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.Api.Tracks
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class TrackQueries
    {
        [UseDbContext(typeof(ApplicationDbContext))]
        [UsePaging]
        public IQueryable<Track> GetTracks(
            [ScopedService] ApplicationDbContext context) 
            => context.Tracks.OrderBy(t => t.Name);

        [UseDbContext(typeof(ApplicationDbContext))]
        public Task<Track> GetTrackByNameAsync(
            string name,
            [ScopedService] ApplicationDbContext context,
            CancellationToken cancellationToken) 
            => context.Tracks.FirstAsync(t => t.Name == name, cancellationToken);

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Track>> GetTrackByNamesAsync(
            string[] names,
            [ScopedService] ApplicationDbContext context,
            CancellationToken cancellationToken) 
            => await context.Tracks.Where(t => names.Contains(t.Name)).ToListAsync(cancellationToken);

        public Task<Track> GetTrackByIdAsync(
            [ID(nameof(Track))] int id,
            TrackByIdDataLoader trackById,
            CancellationToken cancellationToken) 
            => trackById.LoadAsync(id, cancellationToken);

        public async Task<IEnumerable<Track>> GetSessionsByIdAsync(
            [ID(nameof(Track))] int[] ids,
            TrackByIdDataLoader trackById,
            CancellationToken cancellationToken) 
            => await trackById.LoadAsync(ids, cancellationToken);
    }
}
