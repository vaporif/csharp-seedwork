using api.Data.Entities;
using api.Data;
using Microsoft.EntityFrameworkCore;
using api.DataLoader;

namespace api.Users
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class UserQueries
    {
        [UseDbContext(typeof(AppDbContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<User> GetUsers([ScopedService] AppDbContext context) =>
            context.Users;

        public Task<User> GetUserAsync(
            [ID(nameof(User))] int id,
            UserByIdDataLoader dataLoader,
            CancellationToken cancellationToken) =>
            dataLoader.LoadAsync(id, cancellationToken);
    }
}
