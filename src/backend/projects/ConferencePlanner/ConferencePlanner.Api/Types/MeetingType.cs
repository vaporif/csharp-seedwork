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
