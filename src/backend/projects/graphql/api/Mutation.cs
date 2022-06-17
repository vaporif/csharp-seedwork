using api.Data;
using api.Data.Entities;

namespace api
{
    public class Mutation
    {
        public async Task<AddUserPayload> AddUserAsync(
            AddUserInput input,
            [Service] AppDbContext context)
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
