namespace ConferencePlanner.Domain.Entities
{
    public record OrganizerAddedDomainEvent(int MeetingId) : DomainEvent;
}
