using api.Data.Entities;

namespace api
{
    public class AddUserPayload
    {
        public AddUserPayload(User user)
        {
            User = user;
        }

        public User User { get; }
    }
}
