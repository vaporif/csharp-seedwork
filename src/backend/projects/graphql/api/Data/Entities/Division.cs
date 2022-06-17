namespace api.Data.Entities
{
    public class Division
    {
        public int Id { get; set; }

        public string Name { get; set; }

         public ICollection<DivisionEmployee> Employees { get; set; } = 
            new List<DivisionEmployee>();
    }
}
