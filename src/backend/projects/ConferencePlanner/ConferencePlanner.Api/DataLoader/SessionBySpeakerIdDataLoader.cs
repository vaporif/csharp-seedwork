using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.Api.DataLoader
{
    public class SessionBySpeakerIdDataLoader : GroupedDataLoader<int, Session>
    {
        private static readonly string _sessionCacheKey = GetCacheKeyType<SessionByIdDataLoader>();
        private readonly ApplicationDbContext _dbContext;

        public SessionBySpeakerIdDataLoader(
            ApplicationDbContext dbContext,
            IBatchScheduler batchScheduler,
            DataLoaderOptions options)
            : base(batchScheduler, options)
        {
            _dbContext = dbContext ?? 
                throw new ArgumentNullException(nameof(dbContext));
        }

        protected override async Task<ILookup<int, Session>> LoadGroupedBatchAsync(
            IReadOnlyList<int> keys,
            CancellationToken cancellationToken)
        {
            List<SessionSpeaker> list = await _dbContext.Speakers
                .Where(s => keys.Contains(s.Id))
                .Include(s => s.SessionSpeakers)
                .SelectMany(s => s.SessionSpeakers)
                .Include(s => s.Session)
                .ToListAsync(cancellationToken);

            TryAddToCache(_sessionCacheKey, list, item => item.SessionId, item => item.Session!);

            return list.ToLookup(t => t.SpeakerId, t => t.Session!);
        }
    }
}
