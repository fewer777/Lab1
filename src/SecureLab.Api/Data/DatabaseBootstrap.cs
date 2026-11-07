using Microsoft.EntityFrameworkCore;

namespace SecureLab.Api.Data;

public static class DatabaseBootstrap
{
    public static async Task InitializeAsync(
        IServiceProvider services,
        IConfiguration configuration,
        IHostEnvironment environment,
        bool resetRequested)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SecureLabDbContext>();

        if (resetRequested)
        {
            var resetAllowed = configuration.GetValue<bool>("Database:AllowReset");
            if (!environment.IsDevelopment() || !resetAllowed)
            {
                throw new InvalidOperationException(
                    "Reset даних дозволено лише в явно налаштованому Development environment.");
            }

            Console.WriteLine("Відновлення початкових локальних навчальних даних...");
            await dbContext.Database.MigrateAsync();
            // The table names are fixed by the model and never assembled from input.
            // CASCADE also clears later study tables that reference this baseline data.
            await dbContext.Database.ExecuteSqlRawAsync(
                "TRUNCATE TABLE incident_comments, incident_status_history, incidents, study_users RESTART IDENTITY CASCADE;");
            await DbSeeder.SeedAsync(dbContext);
            Console.WriteLine("Локальні навчальні дані очищено та повторно заповнено seed-значеннями.");
            return;
        }

        if (!configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
        {
            return;
        }

        await dbContext.Database.MigrateAsync();
        await DbSeeder.SeedAsync(dbContext);
    }
}
