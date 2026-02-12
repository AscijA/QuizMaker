using Microsoft.EntityFrameworkCore;
using QuizMaker.Infrastructure.Data;

namespace QuizMaker.Api.Extensions;

public static class MigrationDBExtensions {
    public static void ApplyMigrations(this WebApplication app) {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        var dbContext = services.GetRequiredService<QuizDbContext>();

        try {
            logger.LogInformation("Attempting to apply database migrations...");

            if (dbContext.Database.IsRelational()) {
                if (dbContext.Database.GetPendingMigrations().Any()) {
                    dbContext.Database.Migrate();
                    logger.LogInformation("Database migrations applied successfully.");
                }
            }
            else {
                if (!dbContext.Database.CanConnect()) {
                    throw new Exception("Database is unreachable.");
                }
                logger.LogInformation("Database is up to date and reachable.");
            }
        }
        catch (Exception ex) {
            logger.LogCritical(ex, "FATAL: Database migration failed. Application will stop.");
            throw;
        }
    }
}