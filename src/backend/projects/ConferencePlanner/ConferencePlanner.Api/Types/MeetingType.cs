using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Bogus;

namespace ConferencePlanner.GraphQL.Types
{
    public class MeetingType : ObjectType<Meeting>
    {
        protected override void Configure(IObjectTypeDescriptor<Meeting> descriptor)
        {
            descriptor.Field(f => f.Participiants).UseFiltering();
            descriptor.Field("randomFiled").Resolve(ctx => new Faker().Random.Word())
                .Type<NonNullType<StringType>>()
                .Name("randomField");
        }
    }
}
