using api.Data.Entities;
using api.Data;

namespace api
{
    public class Query
    {
        public IQueryable<User> GetUsers([Service] AppDbContext context) =>
            context.Users;
    }
}
