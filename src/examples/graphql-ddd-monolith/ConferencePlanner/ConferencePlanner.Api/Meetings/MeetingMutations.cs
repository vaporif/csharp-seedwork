using ConferencePlanner.Application.Meetings;
using HotChocolate.Subscriptions;

namespace ConferencePlanner.Api.Meetings
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class MeetingMutations
    {
        public async Task<AddMeetingPayload> AddMeetingAsync(
            AddMeetingInput input,
            [Service] AddMeetingCommand command,
            [Service] ITopicEventSender eventSender,
            CancellationToken cancellationToken)
        {
            await command.HandleAsync(input, cancellationToken);
            await eventSender.SendAsync(
                nameof(MeetingSubscriptions.OnMeetingAdded),
                command.Payload!.Meeting, cancellationToken);
            return command.Payload!;
        }

        public async Task<AddMeetingPayload> UpdateMeetingAsync(
            UpdateMeetingInput input,
            [Service] UpdateMeetingCommand command,
            CancellationToken cancellationToken)
        {
            await command.HandleAsync(input, cancellationToken);
            return command.Payload!;
        }
    }
}
