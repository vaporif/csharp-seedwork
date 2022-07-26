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

        public int Id { get; private set; }

        public string? Name { get; private set; }

        public MeetingOrganizer? Organizer { get; private set; }

        public List<Participiant> Participiants { get; private set; } = new List<Participiant>();

        public void SetOrganizer(string firstName, string lastName)
        {
            Organizer = new MeetingOrganizer(firstName, lastName);
        }
    }
}
