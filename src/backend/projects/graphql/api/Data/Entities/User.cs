namespace api.Data.Entities
{
    public class User
    {
        public User(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        private User()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string? Bio { get; set; }

        public ICollection<Post> Posts { get; set; } = new HashSet<Post>();

        public Division? Division { get; set; }

        public int? DivisionId { get; set; }
    }
}
