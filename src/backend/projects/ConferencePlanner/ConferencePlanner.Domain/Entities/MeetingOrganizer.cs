namespace ConferencePlanner.Domain.Entities
{
    public class MeetingOrganizer : SoftDeleteEntity
    {
        private MeetingOrganizer()
        {
        }

        public MeetingOrganizer(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
        
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
    }
}
