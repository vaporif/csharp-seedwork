namespace ConferencePlanner.Domain.Entities
{
    public class Meeting : AggregateRoot
    {
        private Meeting()
        {
        }

        public Meeting(string title)
        {
            Name = title;
        }

        public int Id { get; set; }

        public string? Name { get; set; }

        public MeetingOrganizer? Organizer { get; set; }

        public List<Participiant> Participiants { get; set; } = new List<Participiant>();

        public void SetOrganizer(string firstName, string lastName)
        {
            Organizer = new MeetingOrganizer(firstName, lastName);
            AddDomainEvent(new OrganizerAddedDomainEvent(Id));
        }
    }
}
