using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.Api.Data;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.Api.DataLoader
{
    public class SpeakerBySessionIdDataLoader : GroupedDataLoader<int, Speaker>
    {
        private static readonly string _speakerCacheKey = GetCacheKeyType<SpeakerByIdDataLoader>();
        private readonly ApplicationDbContext _dbContext;

        public SpeakerBySessionIdDataLoader(
            ApplicationDbContext dbContext,
            IBatchScheduler batchScheduler,
            DataLoaderOptions options)
            : base(batchScheduler, options)
        {
            _dbContext = dbContext ?? 
                throw new ArgumentNullException(nameof(dbContext));
        }

        protected override async Task<ILookup<int, Speaker>> LoadGroupedBatchAsync(
            IReadOnlyList<int> keys,
            CancellationToken cancellationToken)
        {
            List<SessionSpeaker> list = await _dbContext.Sessions
                .Where(s => keys.Contains(s.Id))
                .Include(s => s.SessionSpeakers)
                .SelectMany(s => s.SessionSpeakers)
                .Include(s => s.Speaker)
                .ToListAsync(cancellationToken);

            TryAddToCache(_speakerCacheKey, list, item => item.SpeakerId, item => item.Speaker!);

            return list.ToLookup(t => t.SessionId, t => t.Speaker!);
        }
    }
}
