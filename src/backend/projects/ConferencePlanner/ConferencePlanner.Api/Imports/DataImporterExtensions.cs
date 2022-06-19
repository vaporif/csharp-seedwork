

using HotChocolate.Execution.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ConferencePlanner.Api.Imports
{
    public static class ImportRequestExecutorBuilderExtensions
    {
        public static IRequestExecutorBuilder EnsureDatabaseIsCreated(
            this IRequestExecutorBuilder builder) =>
            builder.ConfigureSchemaAsync(async (services, _, ct) =>
            {
                ApplicationDbContext context =
                    services.GetRequiredService<ApplicationDbContext>();
                await using ApplicationDbContext dbContext = context;

                if (await dbContext.Database.EnsureCreatedAsync(ct))
                {
                    var importer = new DataImporter();
                    await importer.LoadDataAsync(dbContext);
                }
            });
        }
}
