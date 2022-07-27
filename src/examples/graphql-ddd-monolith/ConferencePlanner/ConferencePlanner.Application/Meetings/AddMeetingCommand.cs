using ConferencePlanner.Domain.Entities;
namespace ConferencePlanner.Application.Meetings;

public class AddMeetingCommand : ICommand<AddMeetingInput>
{
    private readonly IMeetingsRepository _repo;

    public AddMeetingPayload? Payload { get; private set; }

    public AddMeetingCommand(IMeetingsRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public async ValueTask HandleAsync(AddMeetingInput input, CancellationToken ct = default)
    {
        var meeting = new Meeting(input.Title);
        var result = await _repo.AddAsync(meeting);
        await _repo.SaveChangesAsync(ct);
        Payload = new AddMeetingPayload(result);
    }
}
