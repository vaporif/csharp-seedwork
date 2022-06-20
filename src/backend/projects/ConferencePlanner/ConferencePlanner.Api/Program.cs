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

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb")));

builder.Services
    .AddGraphQLServer()
    .RegisterDbContext<ApplicationDbContext>()
    .AddType<UploadType>()
    .AddQueryType()
    .AddMutationType()
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
    .AddDataLoader<SessionBySpeakerIdDataLoader>()  
    .AddTypeExtension<SpeakerQueries>()
    .AddTypeExtension<SpeakerMutations>()
    // .AddTypeExtension<SpeakerNode>()
    .AddDataLoader<SpeakerByIdDataLoader>()
    .AddDataLoader<SessionBySpeakerIdDataLoader>()   
    .AddTypeExtension<TrackQueries>()
    .AddTypeExtension<TrackMutations>()
    // .AddTypeExtension<TrackNode>()
    .AddDataLoader<TrackByIdDataLoader>()
    .AddType<AttendeeType>()
    .AddType<SessionType>()
    .AddType<SpeakerType>()
    .AddType<TrackType>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    // .EnsureDatabaseIsCreated()
    .AddGlobalObjectIdentification()
    .AddQueryFieldToMutationPayloads()
    // Since we are using subscriptions, we need to register a pub/sub system.
    // for our demo we are using a in-memory pub/sub system.
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
