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

builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"), o => o.UseNodaTime());

        if (!builder.Environment.IsProduction())
        {
            options.EnableSensitiveDataLogging();
        }
    });

builder.Services
    .AddGraphQLServer()
    .RegisterDbContext<ApplicationDbContext>(DbContextKind.Pooled)
    .AddQueryType()
    .AddMutationType()
    .AddSubscriptionType()
        .AddTypeExtension<MeetingQueries>()
        .AddTypeExtension<MeetingMutations>()
        .AddTypeExtension<MeetingSubscriptions>()
    .AddType<MeetingType>()
    .AddDefaultTransactionScopeHandler()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddInMemorySubscriptions();

builder.Services.AddMediatR(typeof(OrganizerAddedDomainEventHandler).GetTypeInfo().Assembly);
builder.Services.AddScoped<IMeetingsRepository, MeetingsRepository>();
builder.Services.AddScoped<AddMeetingCommand>();
builder.Services.AddScoped<UpdateMeetingCommand>();
builder.Services.AddScoped<BoundedContext<ApplicationDbContext>>();
builder.Services.AddSingleton<IClock, SystemClock>();

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

public partial class Program { }
