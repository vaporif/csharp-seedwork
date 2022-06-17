using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotChocolate;
using HotChocolate.Types;
using api.Data.Entities;
using api.Data;
using api.DataLoader;

namespace api.Types
{
    public class UserType : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            descriptor
                .ImplementsNode()
                .IdField(t => t.Id)
                .ResolveNode((ctx, id) => ctx.DataLoader<UserByIdDataLoader>().LoadAsync(id, ctx.RequestAborted));

            descriptor
                .Field(t => t.Division)
                .Resolve(d =>
                {
                    var user = d.Parent<User>();
                    if (user.DivisionId is null)
                        return null;

                    return d.BatchDataLoader<int, Division>(
                        async (keys, ct) =>
                        {
                            return await d.DbContext<AppDbContext>().Divisions.Where(s => keys.Contains(s.Id)).ToDictionaryAsync(x => x.Id, v => v);
                        }).LoadAsync(user.DivisionId);
                })
                .UseDbContext<AppDbContext>()
                .Name(nameof(Division).ToLower());
        }

    }
}
