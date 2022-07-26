using ConferencePlanner.Domain.Entities;
namespace ConferencePlanner.Application.Meetings;
public class OrganizerAddedDomainEventHandler : IDomainEventHandler<OrganizerAddedDomainEvent>
{
    private readonly IMeetingsRepository _repo;

    public OrganizerAddedDomainEventHandler(IMeetingsRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public async Task Handle(OrganizerAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        var meeting = await _repo.FindAsync(notification.MeetingId);
        meeting!.Organizer!.FirstName = $"{meeting.Organizer.FirstName}_postEvent";
    }
}
