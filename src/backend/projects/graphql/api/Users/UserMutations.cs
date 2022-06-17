using api.Data;
using api.Data.Entities;
using api.Extensions;

namespace api
{
    [ExtendObjectType("Mutation")]
    public class UserMutations
    {
        [UseApplicationDbContext]
        public async Task<AddUserPayload> AddUserAsync(
            AddUserInput input,
            [ScopedService] AppDbContext context)
        {
            var speaker = new User(input.Name)
            {
                Bio = input.Bio
            };

            var division = await context.Divisions.FindAsync(input.DivisionId);

            speaker.Division = division;

            await context.Users.AddAsync(speaker);
            await context.SaveChangesAsync();

            return new AddUserPayload(speaker);
        }
    }
}
