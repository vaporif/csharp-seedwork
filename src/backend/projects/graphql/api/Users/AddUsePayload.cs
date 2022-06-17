using api.Common;
using api.Data.Entities;
using api.Users;

namespace api
{
    public class AddUserPayload : UserPayloadBase
    {
        public AddUserPayload(User user) : base(user)
        {
        }

        public AddUserPayload(IReadOnlyList<UserError> errors) : base(errors)
        {
        }
    }
}
