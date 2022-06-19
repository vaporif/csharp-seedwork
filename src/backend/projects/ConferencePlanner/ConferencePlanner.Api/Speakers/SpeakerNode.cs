using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.Api.Data;
using ConferencePlanner.Api.DataLoader;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.Api.Speakers
{
    [Node]
    [ExtendObjectType(typeof(Speaker))]
    public class SpeakerNode
    {
        [BindMember(nameof(Speaker.Bio), Replace = true)]
        public string? GetBio([Parent] Speaker speaker, bool error = false)
        {
            if(error) 
            {
                throw new GraphQLException("Some error with the bio.");
            }

            return speaker.Bio;
        }

        [BindMember(nameof(Speaker.SessionSpeakers), Replace = true)]
        public async Task<IEnumerable<Session>> GetSessionsAsync(
            [Parent] Speaker speaker,
            SessionBySpeakerIdDataLoader sessionBySpeakerId,
            CancellationToken cancellationToken)
            => await sessionBySpeakerId.LoadAsync(speaker.Id, cancellationToken);

        [NodeResolver]
        public static Task<Speaker> GetSpeakerByIdAsync(
            int id,
            SpeakerByIdDataLoader speakerById,
            CancellationToken cancellationToken)
            => speakerById.LoadAsync(id, cancellationToken);

        public async Task<IEnumerable<Session>> GetSessionsExpensiveAsync(
            [Parent] Speaker speaker,
            SessionBySpeakerIdDataLoader sessionBySpeakerId,
            CancellationToken cancellationToken)
        {
            await Task.Delay(new Random().Next(1000, 3000), cancellationToken);

            return await sessionBySpeakerId.LoadAsync(speaker.Id, cancellationToken);
        }

        public async IAsyncEnumerable<Session> GetSessionsStreamAsync(
            [Parent] Speaker speaker,
            ApplicationDbContext dbContext,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var random = new Random();

            await Task.Delay(random.Next(500, 1000), cancellationToken);

            var stream = (IAsyncEnumerable<SessionSpeaker>)dbContext.Speakers
                .Where(s => s.Id == speaker.Id)
                .Include(s => s.SessionSpeakers)
                .SelectMany(s => s.SessionSpeakers)
                .Include(s => s.Session);

            await foreach (var item in stream.WithCancellation(cancellationToken))
            {
                if (item.Session is not null)
                {
                    yield return item.Session;
                }

                await Task.Delay(random.Next(100, 300), cancellationToken);
            }
        }
    }
}