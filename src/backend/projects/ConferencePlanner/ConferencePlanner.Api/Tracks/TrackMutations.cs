using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.Api.Data;
using HotChocolate;
using HotChocolate.Types;

namespace ConferencePlanner.Api.Tracks
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class TrackMutations
    {
        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<AddTrackPayload> AddTrackAsync(
            AddTrackInput input,
            [ScopedService] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var track = new Track { Name = input.Name };
            context.Tracks.Add(track);

            await context.SaveChangesAsync(cancellationToken);

            return new AddTrackPayload(track);
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<RenameTrackPayload> RenameTrackAsync(
            RenameTrackInput input,
            [ScopedService] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var track = await context.Tracks.FindAsync(input.Id, cancellationToken);

            if (track is null)
            {
                throw new GraphQLException("Track not found.");
            }
            
            track.Name = input.Name;

            await context.SaveChangesAsync(cancellationToken);

            return new RenameTrackPayload(track);
        }
    }
}
