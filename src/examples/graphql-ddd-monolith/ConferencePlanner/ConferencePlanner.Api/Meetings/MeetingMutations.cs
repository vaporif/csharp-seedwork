using ConferencePlanner.Application.Meetings;
using HotChocolate.Subscriptions;

namespace ConferencePlanner.Api.Meetings
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class MeetingMutations
    {
        public async Task<AddMeetingPayload> AddMeetingAsync(
            AddMeetingInput input,
            CancellationToken cancellationToken,
            [Service] AddMeetingCommand command,
            [Service] ITopicEventSender eventSender)
        {
            await command.HandleAsync(input, cancellationToken);
            await eventSender.SendAsync(
                nameof(MeetingSubscriptions.OnMeetingAdded),
                command.Payload!.Meeting);
            return command.Payload!;
        }

        public async Task<AddMeetingPayload> UpdateMeetingAsync(
            UpdateMeetingInput input,
            CancellationToken cancellationToken,
            [Service] UpdateMeetingCommand command)
        {
            await command.HandleAsync(input, cancellationToken);
            return command.Payload!;
        }
    }
}
