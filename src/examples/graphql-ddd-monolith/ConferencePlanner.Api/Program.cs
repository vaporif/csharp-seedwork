global using ConferencePlanner.Domain.Entities;
global using ConferencePlanner.Infrastructure;
using System.Reflection;
using Bogus;
using ConferencePlanner.Api.Meetings;
using ConferencePlanner.Application.Meetings;
using ConferencePlanner.GraphQL.Types;
using ConferencePlanner.Infrastructure.Meetings;
using HealthChecks.UI.Client;
using HotChocolate.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Serilog;

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

// NOTE: Used in graphQl
builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(RegisterDbContext);

// NOTE: Used for healthcheck
builder.Services.AddDbContext<ApplicationDbContext>(RegisterDbContext);

void RegisterDbContext(DbContextOptionsBuilder options)
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"), o => o.UseRelationalNulls(true));

    if (!builder.Environment.IsProduction())
    {
        options.EnableSensitiveDataLogging();
    }
}

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
    .AddInMemorySubscriptions()
    .AddApolloTracing()
    .AddInstrumentation(o => o.Scopes = ActivityScopes.All);

builder.Logging.AddOpenTelemetry(
    b =>
    {
        b.IncludeFormattedMessage = true;
        b.IncludeScopes = true;
        b.ParseStateValues = true;
        b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ConferencePlanner"));
    });

builder.Services.AddOpenTelemetryTracing(
    b =>
    {
        b.AddHttpClientInstrumentation();
        b.AddAspNetCoreInstrumentation();
        b.AddHotChocolateInstrumentation();
        b.AddJaegerExporter();
    });

builder.Services.AddMediatR(typeof(OrganizerAddedDomainEventHandler).GetTypeInfo().Assembly);
builder.Services.AddScoped<IMeetingsRepository, MeetingsRepository>();
builder.Services.AddScoped<AddMeetingCommand>();
builder.Services.AddScoped<UpdateMeetingCommand>();
builder.Services.AddScoped<BoundedContext<ApplicationDbContext>>();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
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
        await using var dbContext = context.CreateDbContext();

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
