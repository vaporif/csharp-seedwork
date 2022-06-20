namespace ConferencePlanner.Domain.Entities
{
    public class Participiant
    {
        public int Id {get;set;}
        
        public string? FirstName {get;set;}

        public string? LastName {get;set;}

        public List<Meeting> Meetings {get;set;} = new List<Meeting>();
    }
}
