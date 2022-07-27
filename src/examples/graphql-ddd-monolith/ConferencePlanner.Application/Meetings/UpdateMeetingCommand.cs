namespace ConferencePlanner.Application.Meetings
{
    using ConferencePlanner.Domain.Entities;

    public class UpdateMeetingCommand : ICommand<UpdateMeetingInput>
    {
        private readonly IMeetingsRepository _repo;

        public AddMeetingPayload? Payload { get; private set; }

        public UpdateMeetingCommand(IMeetingsRepository repo) => _repo = repo ?? throw new ArgumentNullException(nameof(repo));

        public async ValueTask HandleAsync(UpdateMeetingInput input, CancellationToken ct = default)
        {
            var meeting = await _repo.FindAsync(input.Id);
            if (meeting is null)
            {
                throw DomainExceptionEntityNotFound.Create(nameof(Meeting), input.Id);
            }


            if (input.Organizer is not null)
            {
                meeting.SetOrganizer(input.Organizer.FirstName!, input.Organizer.LastName!);
            }

            if (input.Name is not null)
            {
                meeting.Name = input.Name;
            }

            await _repo.SaveChangesAsync(ct);
            Payload = new AddMeetingPayload(meeting);
        }
    }
}