using Microsoft.EntityFrameworkCore;
using SecureLab.Api.Data.Entities;

namespace SecureLab.Api.Data;

public static class DbSeeder
{
    public static readonly Guid AliceId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    public static readonly Guid BobId = Guid.Parse("10000000-0000-0000-0000-000000000002");
    public static readonly Guid MorganId = Guid.Parse("10000000-0000-0000-0000-000000000003");
    public static readonly Guid AdminId = Guid.Parse("10000000-0000-0000-0000-000000000004");

    public static readonly Guid AliceIncidentId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    public static readonly Guid BobIncidentId = Guid.Parse("20000000-0000-0000-0000-000000000002");

    public static async Task SeedAsync(SecureLabDbContext dbContext)
    {
        if (await dbContext.Users.AnyAsync())
        {
            return;
        }

        var alice = new StudyUser
        {
            Id = AliceId,
            UserName = "alice",
            DisplayName = "Аліса Коваль",
            Email = "alice@example.test",
            Role = "Reporter"
        };
        var bob = new StudyUser
        {
            Id = BobId,
            UserName = "bob",
            DisplayName = "Боб Мельник",
            Email = "bob@example.test",
            Role = "Reporter"
        };
        var morgan = new StudyUser
        {
            Id = MorganId,
            UserName = "morgan",
            DisplayName = "Морган Литвин",
            Email = "morgan@example.test",
            Role = "Analyst"
        };
        var admin = new StudyUser
        {
            Id = AdminId,
            UserName = "admin",
            DisplayName = "Локальний адміністратор",
            Email = "admin@example.test",
            Role = "Administrator"
        };

        var aliceIncident = new Incident
        {
            Id = AliceIncidentId,
            Owner = alice,
            Title = "Підозрілий лист із вкладенням",
            Description = "Одержано лист нібито від деканату. Вкладення не відкривалося.",
            Severity = IncidentSeverity.Medium,
            Status = IncidentStatus.Triaged,
            OccurredAtUtc = new DateTimeOffset(2026, 8, 1, 8, 30, 0, TimeSpan.Zero),
            CreatedAtUtc = new DateTimeOffset(2026, 8, 1, 9, 0, 0, TimeSpan.Zero),
            UpdatedAtUtc = new DateTimeOffset(2026, 8, 1, 10, 0, 0, TimeSpan.Zero)
        };
        var bobIncident = new Incident
        {
            Id = BobIncidentId,
            Owner = bob,
            Title = "Невідома спроба входу",
            Description = "Система зафіксувала вхід до облікового запису з нового пристрою.",
            Severity = IncidentSeverity.High,
            Status = IncidentStatus.InProgress,
            OccurredAtUtc = new DateTimeOffset(2026, 8, 2, 14, 10, 0, TimeSpan.Zero),
            CreatedAtUtc = new DateTimeOffset(2026, 8, 2, 14, 20, 0, TimeSpan.Zero),
            UpdatedAtUtc = new DateTimeOffset(2026, 8, 2, 15, 0, 0, TimeSpan.Zero)
        };
        var safeTextIncident = new Incident
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
            Owner = alice,
            Title = "Перевірка журналу комп'ютерного класу",
            Description = "Текст <script> має відображатися як текст, а не виконуватися як HTML.",
            Severity = IncidentSeverity.Low,
            Status = IncidentStatus.New,
            OccurredAtUtc = new DateTimeOffset(2026, 8, 3, 7, 45, 0, TimeSpan.Zero),
            CreatedAtUtc = new DateTimeOffset(2026, 8, 3, 8, 0, 0, TimeSpan.Zero),
            UpdatedAtUtc = new DateTimeOffset(2026, 8, 3, 8, 0, 0, TimeSpan.Zero)
        };

        dbContext.Users.AddRange(alice, bob, morgan, admin);
        dbContext.Incidents.AddRange(aliceIncident, bobIncident, safeTextIncident);
        dbContext.IncidentComments.AddRange(
            new IncidentComment
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                Incident = aliceIncident,
                Author = morgan,
                Text = "Заголовки листа передано аналітику.",
                IsInternal = false,
                CreatedAtUtc = new DateTimeOffset(2026, 8, 1, 9, 30, 0, TimeSpan.Zero)
            },
            new IncidentComment
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                Incident = bobIncident,
                Author = morgan,
                Text = "Розпочато перевірку журналу автентифікації.",
                IsInternal = false,
                CreatedAtUtc = new DateTimeOffset(2026, 8, 2, 14, 40, 0, TimeSpan.Zero)
            });
        dbContext.IncidentStatusHistory.Add(
            new IncidentStatusHistory
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                Incident = aliceIncident,
                ChangedByUser = morgan,
                OldStatus = IncidentStatus.New,
                NewStatus = IncidentStatus.Triaged,
                Note = "Первинну оцінку завершено.",
                CreatedAtUtc = new DateTimeOffset(2026, 8, 1, 10, 0, 0, TimeSpan.Zero)
            });

        await dbContext.SaveChangesAsync();
    }
}
