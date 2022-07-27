namespace ConferencePlanner.Application.Meetings
{
    public record UpdateMeetingInput(int Id, string? Name, Organizer? Organizer);

    public record Organizer(string FirstName, string LastName);
}