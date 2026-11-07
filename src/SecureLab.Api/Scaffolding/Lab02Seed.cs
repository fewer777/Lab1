using Microsoft.EntityFrameworkCore;
using SecureLab.Api.Data;
using SecureLab.Api.Data.Entities;

namespace SecureLab.Api.Scaffolding;

public static class Lab02Seed
{
    public static async Task EnsureAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<SecureLabDbContext>();
        for (var number = 4; number <= 5; number++)
        {
            var id = Guid.Parse($"20000000-0000-0000-0000-{number:000000000000}");
            if (await db.Incidents.AnyAsync(row => row.Id == id)) continue;
            var now = new DateTimeOffset(2026, 8, 3, 9, 0, 0, TimeSpan.Zero);
            db.Incidents.Add(new Incident
            {
                Id = id, OwnerUserId = number == 4 ? DbSeeder.BobId : DbSeeder.AliceId,
                Title = number == 4 ? "O'Brien classroom report" : "Перевірка USB навчальної мережі",
                Description = "Навчальний запис для перевірки пошуку без реальних персональних даних.",
                Severity = IncidentSeverity.Low, Status = IncidentStatus.New,
                OccurredAtUtc = now, CreatedAtUtc = now, UpdatedAtUtc = now
            });
        }
        await db.SaveChangesAsync();
    }
}
