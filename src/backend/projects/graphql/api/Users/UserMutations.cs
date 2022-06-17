using api.Data;
using api.Data.Entities;

namespace api
{
    [ExtendObjectType("Mutation")]
    public class UserMutations
    {
        [UseDbContext(typeof(AppDbContext))]
        public async Task<AddUserPayload> AddUserAsync(
            AddUserInput input,
            [ScopedService] AppDbContext context)
        {
            var speaker = new User(input.Name)
            {
                Bio = input.Bio
            };

            await context.Users.AddAsync(speaker);
            await context.SaveChangesAsync();

            return new AddUserPayload(speaker);
        }
    }
}
