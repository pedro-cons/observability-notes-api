using Microsoft.EntityFrameworkCore;
using Notes.API.Database;

namespace Notes.API.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app, ILogger logger)
    {
		try
		{
            logger.LogInformation("Starting database migrations...");

            using var scope = app.ApplicationServices.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Database.Migrate();

            logger.LogInformation("Database migrations completed.");
        }
		catch (Exception ex)
		{
            logger.LogError(ex, "An error occurred while applying database migrations.");
        }
    }
}
