using System.Collections.Generic;
using api.Common;
using api.Data.Entities;

namespace api.Users
{
    public class UserPayloadBase : Payload
    {
        protected UserPayloadBase(User user)
        {
            User = user;
        }

        protected UserPayloadBase(IReadOnlyList<UserError> errors)
            : base(errors)
        {
        }

        public User? User { get; }
    }
}
