using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConferencePlanner.Api.DataLoader;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace ConferencePlanner.GraphQL.Types
{
    public class ParticipiantType : ObjectType<Participiant>
    {
        protected override void Configure(IObjectTypeDescriptor<Participiant> descriptor)
        {
            descriptor.Field(f => f.Meetings).UseOffsetPaging();
        }
    }
}
