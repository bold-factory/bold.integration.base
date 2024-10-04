using Microsoft.EntityFrameworkCore;

namespace Bold.Integration.Base.Persistence;

public static class MigrationsApplier
{
    public static WebApplication ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Starting migration...");
        logger.LogInformation("Applying migration to database...");
        var itemsDbContext = scope.ServiceProvider.GetRequiredService<IntegrationDbContext>();
        itemsDbContext.Database.SetCommandTimeout(120);
        itemsDbContext.Database.Migrate();
        logger.LogInformation("All migrations correctly applied.");
        return app;
    }
}