using api.Data.Entities;
using api.Data;
using api.Extensions;
using Microsoft.EntityFrameworkCore;
using api.DataLoader;

namespace api
{
    public class Query
    {
        [UseApplicationDbContext]
        [UsePaging]
        public IQueryable<User> GetUsersAsync([ScopedService] AppDbContext context) =>
            context.Users;

        [UseApplicationDbContext]
        public Task<List<Division>> GetDivisionsAsync([ScopedService] AppDbContext context) =>
            context.Divisions.ToListAsync();

        public Task<User> GetUserAsync(
            [ID(nameof(User))] int id,
            UserByIdDataLoader dataLoader,
            CancellationToken cancellationToken) =>
            dataLoader.LoadAsync(id, cancellationToken);


        public Task<Division> GetDivisionAsync(
            int id,
            DivisionByIdDataLoader dataLoader,
            CancellationToken cancellationToken) =>
            dataLoader.LoadAsync(id, cancellationToken);
    }
}
