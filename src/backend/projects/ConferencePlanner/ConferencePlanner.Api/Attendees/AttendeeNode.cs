// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using ConferencePlanner.Api.Data;
// using ConferencePlanner.Api.DataLoader;
// using HotChocolate;
// using HotChocolate.Types;
// using HotChocolate.Types.Relay;
// using Microsoft.EntityFrameworkCore;

// namespace ConferencePlanner.Api.Attendees
// {
//     [Node]
//     [ExtendObjectType]
//     public class AttendeeNode
//     {
//         public async Task<IEnumerable<Session>> GetSessionsAsync(
//             [Parent] Attendee attendee,
//             ApplicationDbContext dbContext,
//             SessionByIdDataLoader sessionById,
//             CancellationToken cancellationToken)
//         {
//             int[] speakerIds = await dbContext.Attendees
//                 .Where(a => a.Id == attendee.Id)
//                 .Include(a => a.SessionsAttendees)
//                 .SelectMany(a => a.SessionsAttendees.Select(t => t.SessionId))
//                 .ToArrayAsync(cancellationToken);

//             return await sessionById.LoadAsync(speakerIds, cancellationToken);
//         }

//         [NodeResolver]
//         public static Task<Attendee> GetAttendeeAsync(
//             int id,
//             AttendeeByIdDataLoader attendeeById,
//             CancellationToken cancellationToken)
//             => attendeeById.LoadAsync(id, cancellationToken);
//     }
// }
