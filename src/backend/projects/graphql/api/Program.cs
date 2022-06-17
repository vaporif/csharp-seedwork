using api;
using api.Data;
using api.DataLoader;
using api.Types;
using api.Users;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
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

builder.Services.AddPooledDbContextFactory<AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb")));

builder.Services
    .AddGraphQLServer()
    .AddQueryType()
    .AddMutationType()
    .AddTypeExtension<UserQueries>()
    .AddTypeExtension<UserMutations>()
    // .AddType<UserType>()
    // .AddGlobalObjectIdentification()
    .AddQueryFieldToMutationPayloads()
    .AddDataLoader<UserByIdDataLoader>()
    .AddFiltering()
    .AddSorting()
    .AddProjections();

builder.Services.AddErrorFilter<GraphErrorFilter>();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
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
