global using ConferencePlanner.Infrastructure;
global using ConferencePlanner.Domain.Entities;
//using ConferencePlanner.GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Bogus;
using ConferencePlanner.Api.Meetings;
using ConferencePlanner.Application.Meetings;
using ConferencePlanner.Infrastructure.Meetings;
using Prometheus;
using MediatR;
using System.Reflection;
using HotChocolate.Types.NodaTime;
using HotChocolate.Execution.Configuration;
using ConferencePlanner.GraphQL.Types;

Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
Serilog.Debugging.SelfLog.Enable(Console.Error);
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console());

builder.Services.AddDbContextFactory<ApplicationDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"), o => o.UseNodaTime().EnableRetryOnFailure());

        if (!builder.Environment.IsProduction())
        {
            options.EnableSensitiveDataLogging();
        }
    });

builder.Services
    .AddGraphQLServer()
    .RegisterDbContext<ApplicationDbContext>(DbContextKind.Synchronized)
    .AddQueryType()
    .AddMutationType()
    .AddSubscriptionType()
        .AddTypeExtension<MeetingQueries>()
        .AddTypeExtension<MeetingMutations>()
        .AddTypeExtension<MeetingSubscriptions>()
    .AddType<MeetingType>()
    .AddType<InstantType>()
    .AddDefaultTransactionScopeHandler()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddInMemorySubscriptions();

builder.Services.AddMediatR(typeof(Program).GetTypeInfo().Assembly);
builder.Services.AddScoped<IMeetingsRepository, MeetingsRepository>();
builder.Services.AddScoped<AddMeetingCommand>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database")
    .ForwardToPrometheus();

var app = builder.Build();

app.UseRouting();
app.UseHttpMetrics();

app.UseWebSockets();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    endpoints.MapGraphQL();
});

try
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        await using ApplicationDbContext dbContext = context.CreateDbContext();

        if (await dbContext.Database.EnsureCreatedAsync())
        {
            if (!dbContext.Meetings.Any())
            {
                Log.Information("generating fake data");
                var fakerPart = new Faker<Participiant>()
                    .RuleFor(f => f.FirstName, f => f.Name.FirstName())
                    .RuleFor(d => d.LastName, f => f.Name.LastName());
                var faker = new Faker<Meeting>()
                    .CustomInstantiator(f => new Meeting(f.Company.CatchPhrase()))
                    .RuleFor(d => d.Id, 0)
                    .RuleFor(d => d.Organizer, f => new MeetingOrganizer(f.Person.FirstName, f.Person.LastName))
                    .RuleFor(d => d.Participiants, f => fakerPart.GenerateBetween(2, 20));

                var meetings = faker.Generate(200);
                await dbContext.Meetings.AddRangeAsync(meetings);
                await dbContext.SaveChangesAsync();
            }
        }
    }
    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

public static class SchemaExtensions
{
    public static IRequestExecutorBuilder AddNodaTime(
                this IRequestExecutorBuilder schemaBuilder,
                params Type[] excludeTypes)
    {
        var nodaTimeTypes = new[]
        {
            typeof(DateTimeZoneType), typeof(DurationType), typeof(InstantType),
            typeof(IsoDayOfWeekType), typeof(LocalDateTimeType), typeof(LocalDateType),
            typeof(LocalTimeType), typeof(OffsetDateTimeType), typeof(OffsetDateType),
            typeof(OffsetTimeType), typeof(OffsetType), typeof(PeriodType),
            typeof(ZonedDateTimeType),
        };
        foreach (var type in nodaTimeTypes.Except(excludeTypes))
        {
            schemaBuilder = schemaBuilder.AddType(type);
        }

        return schemaBuilder;
    }
}


public partial class Program { }
