using HotChocolate.Subscriptions;

namespace ConferencePlanner.Api.Meetings
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class MeetingMutations
    {
        public async Task<AddMeetingPayload> AddMeetingAsync(
            AddMeetingInput input,
            ApplicationDbContext context,
            CancellationToken cancellationToken,
            [Service]ITopicEventSender eventSender)
        {
            var meeting = new Meeting { Name = input.Name };
            context.Add(meeting);
            await context.SaveChangesAsync(cancellationToken);
            await eventSender.SendAsync(
                nameof(MeetingSubscriptions.OnMeetingAdded),
                meeting);
            return new AddMeetingPayload(meeting);
        }
    }
}
