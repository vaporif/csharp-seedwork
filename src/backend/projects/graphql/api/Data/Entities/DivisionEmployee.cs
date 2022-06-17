namespace api.Data.Entities
{
    public class DivisionEmployee
    {
        public int DivisionId { get; set; }

        public Division? Division { get; set; }

        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }
    }
}
