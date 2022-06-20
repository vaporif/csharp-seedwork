namespace ConferencePlanner.Domain.Entities
{
    public class Meeting
    {
        public int Id {get;set;}

        public string? Name {get;set;}

        public MeetingOrganizer? Organizer {get;set;}

        public List<Participiant> Participiants {get;set;} = new List<Participiant>();
    }
}
