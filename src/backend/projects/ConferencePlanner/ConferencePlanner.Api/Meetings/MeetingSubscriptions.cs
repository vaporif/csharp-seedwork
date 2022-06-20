namespace ConferencePlanner.Api.Meetings
{
    [ExtendObjectType(OperationTypeNames.Subscription)]
    public class MeetingSubscriptions
    {
        [Subscribe]
        [Topic]
        public Meeting OnMeetingAdded(
            [EventMessage] Meeting meeting,
            CancellationToken cancellationToken) =>
            meeting;
    }
}
