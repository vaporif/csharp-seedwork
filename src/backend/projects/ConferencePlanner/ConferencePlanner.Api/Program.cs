global using ConferencePlanner.Infrastructure;
global using ConferencePlanner.Domain.Entities;

using ConferencePlanner.Api;
using ConferencePlanner.Api.Attendees;
using ConferencePlanner.Api.DataLoader;
using ConferencePlanner.Api.Imports;
using ConferencePlanner.Api.Sessions;
using ConferencePlanner.Api.Speakers;
using ConferencePlanner.Api.Tracks;
using ConferencePlanner.GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Authorization;
using HealthChecks.UI.Client;
using Bogus;
using ConferencePlanner.Api.Meetings;

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
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb")));

builder.Services
    .AddGraphQLServer()
    .RegisterDbContext<ApplicationDbContext>(DbContextKind.Pooled)
    .AddType<UploadType>()
    .AddQueryType()
    .AddMutationType()
    .AddSubscriptionType()
    .AddTypeExtension<AttendeeQueries>()
    .AddTypeExtension<AttendeeMutations>()
    .AddTypeExtension<AttendeeSubscriptions>()
    // .AddTypeExtension<AttendeeNode>()
    .AddDataLoader<AttendeeByIdDataLoader>()
    .AddTypeExtension<SessionQueries>()
    .AddTypeExtension<SessionMutations>()
    .AddTypeExtension<SessionSubscriptions>()
    // .AddTypeExtension<SessionNode>() 
    .AddDataLoader<SessionByIdDataLoader>()
    .AddTypeExtension<SpeakerQueries>()
    .AddTypeExtension<SpeakerMutations>()
    .AddTypeExtension<MeetingQueries>()
    .AddTypeExtension<MeetingMutations>()
    .AddTypeExtension<MeetingSubscriptions>()
    .AddTypeExtension<SessionSubscriptions>()
    .AddTypeExtension<AttendeeSubscriptions>()
    // .AddTypeExtension<SpeakerNode>()
    .AddDataLoader<SpeakerByIdDataLoader>() 
    .AddTypeExtension<TrackQueries>()
    .AddTypeExtension<TrackMutations>()
    // .AddTypeExtension<TrackNode>()
    .AddDataLoader<TrackByIdDataLoader>()
    .AddType<AttendeeType>()
    .AddType<SessionType>()
    .AddType<SpeakerType>()
    .AddType<MeetingType>()
    .AddType<TrackType>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    // .EnsureDatabaseIsCreated()
    .AddGlobalObjectIdentification()
    .AddQueryFieldToMutationPayloads()
    // Since we are using subscriptions, we need to register a pub/sub system.
    // for our demo we are using a in-memory pub/sub system.
    .InitializeOnStartup()
    .AddInMemorySubscriptions();
    // Last we add support for persisted queries. 
    // The first line adds the persisted query storage, 
    // the second one the persisted query processing pipeline.
    // .QuerySt
    // .AddFileSystemQueryStorage("./persisted_queries")
    // .UsePersistedQueryPipeline();

builder.Services.AddHealthChecks();
    

builder.Services.AddErrorFilter<GraphErrorFilter>();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
     ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseSerilogRequestLogging().UseRouting();

app.UseRouting();

app.UseWebSockets();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();

    endpoints.MapHealthChecks("/health", new HealthCheckOptions()
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }).WithMetadata(new AllowAnonymousAttribute());
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
            var importer = new DataImporter();
            await importer.LoadDataAsync(dbContext);

            if(!dbContext.Meetings.Any())
            {
                Log.Information("generating fake data");
                var fakerPart = new Faker<Participiant>()
                    .RuleFor(f => f.FirstName, f => f.Name.FirstName())
                    .RuleFor(d => d.LastName, f => f.Name.LastName());
                var faker = new Faker<Meeting>()
                    .RuleFor(d => d.Id, 0)
                    .RuleFor(d => d.Name, f => f.Company.CatchPhrase())
                    .RuleFor(d => d.Organizer, f => new MeetingOrganizer{ FirstName = f.Person.FirstName, LastName = f.Person.LastName })
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
