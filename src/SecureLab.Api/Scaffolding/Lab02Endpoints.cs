using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SecureLab.Api.Data;
using SecureLab.Api.Data.Entities;

namespace SecureLab.Api.Scaffolding;

public static class Lab02Endpoints
{
    public static void MapLab02Endpoints(this WebApplication app)
    {
        // ------------------------------------------------------------------
        // ЕТАП 2: POST /api/incidents — Серверна валідація та DTO
        // ------------------------------------------------------------------
        app.MapPost("/api/incidents", async (
            CreateIncidentRequest request,
            SecureLabDbContext db,
            CancellationToken ct) =>
        {
            var now = DateTimeOffset.UtcNow;
            var errors = new Dictionary<string, string[]>();

            // 1. Перевірка Title (Обов'язковий, max 160 символів)
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                errors["title"] = ["Title is required."];
            }
            else if (request.Title.Length > 160)
            {
                errors["title"] = ["Max title length is 160 chars."];
            }

            // 2. Перевірка Description (Обов'язковий, max 4000 символів)
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                errors["description"] = ["Description is required."];
            }
            else if (request.Description.Length > 4000)
            {
                errors["description"] = ["Max description length is 4000 chars."];
            }

            // 3. Перевірка Severity (Enum: Low, Medium, High, Critical)
            IncidentSeverity severity = default;
            var severityIsValid = !string.IsNullOrWhiteSpace(request.Severity)
                                  && Enum.TryParse<IncidentSeverity>(request.Severity, ignoreCase: true, out severity)
                                  && Enum.IsDefined(severity);

            if (!severityIsValid)
            {
                errors["severity"] = ["Allowed values: Low, Medium, High, Critical."];
            }

            // 4. Перевірка OccurredAtUtc (Обов'язковий, не більше +5 хв у майбутнє)
            if (request.OccurredAtUtc is null)
            {
                errors["occurredAtUtc"] = ["OccurredAtUtc is required."];
            }
            else if (request.OccurredAtUtc.Value > now.AddMinutes(5))
            {
                errors["occurredAtUtc"] = ["Date cannot be more than 5 minutes ahead of server UTC time."];
            }

            // 5. Cross-field перевірка для High / Critical (min 40 символів в описі)
            var normalizedTitle = request.Title?.Trim() ?? string.Empty;
            var normalizedDescription = request.Description?.Trim() ?? string.Empty;

            if (severityIsValid && (severity == IncidentSeverity.High || severity == IncidentSeverity.Critical))
            {
                if (!errors.ContainsKey("description") && normalizedDescription.Length < 40)
                {
                    errors["description"] = ["Description for High/Critical must be at least 40 chars long."];
                }
            }

            // Повертаємо 400 ValidationProblem за наявності помилок
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            // 6. Перевірка предметного конфлікту 409 (дублікат Title в активних статусах)
            var activeStatuses = new[]
            {
                IncidentStatus.New,
                IncidentStatus.Triaged,
                IncidentStatus.InProgress,
                IncidentStatus.Resolved
            };

            var hasDuplicateTitle = await db.Incidents.AnyAsync(
                i => i.Title == normalizedTitle && activeStatuses.Contains(i.Status),
                ct);

            if (hasDuplicateTitle)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Incident Title Conflict",
                    detail: "An active incident with the same title already exists.");
            }

            // 7. Створення сутності (Server-managed поля)
            var incident = new Incident
            {
                Id = Guid.NewGuid(),
                Title = normalizedTitle,
                Description = normalizedDescription,
                Severity = severity,
                Status = IncidentStatus.New,
                OccurredAtUtc = request.OccurredAtUtc!.Value.ToUniversalTime(),
                CreatedAtUtc = now,
                UpdatedAtUtc = now,
                OwnerUserId = DbSeeder.AliceId
            };

            db.Incidents.Add(incident);
            await db.SaveChangesAsync(ct);

            // 8. DTO відповіді (201 Created)
            var response = new CreatedIncidentResponse(
                incident.Id,
                incident.Title,
                incident.Description,
                incident.Severity.ToString(),
                incident.Status.ToString(),
                incident.OccurredAtUtc,
                incident.CreatedAtUtc);

            return Results.Created($"/api/incidents/{incident.Id}", response);
        });

        // ------------------------------------------------------------------
        // ЕТАП 3-4: GET /api/incidents/search — Безпечний пошук через LINQ
        // ------------------------------------------------------------------
        app.MapGet("/api/incidents/search", async (
            string? q,
            string? sortBy,
            SecureLabDbContext db,
            CancellationToken ct) =>
        {
            // Перевірка allowlist для sortBy
            if (sortBy is not (null or "" or "createdAtUtc" or "severity" or "status"))
            {
                return Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        ["sortBy"] = ["Allowed values: createdAtUtc, severity, status."]
                    });
            }

            // Екранування спецсимволів SQL ILIKE (\, %, _)
            var search = (q ?? "")
                .Replace("\\", "\\\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_");

            var query = db.Incidents
                .AsNoTracking()
                .Where(i =>
                    EF.Functions.ILike(i.Title, "%" + search + "%", "\\") ||
                    EF.Functions.ILike(i.Description, "%" + search + "%", "\\"));

            // Сортування через Allowlist та Ранги
            IQueryable<Incident> sortedQuery = sortBy switch
            {
                "severity" => query
                    .OrderBy(i => i.Severity == IncidentSeverity.Critical ? 0 :
                                  i.Severity == IncidentSeverity.High ? 1 :
                                  i.Severity == IncidentSeverity.Medium ? 2 : 3)
                    .ThenBy(i => i.Id),

                "status" => query
                    .OrderBy(i => i.Status == IncidentStatus.New ? 0 :
                                  i.Status == IncidentStatus.Triaged ? 1 :
                                  i.Status == IncidentStatus.InProgress ? 2 :
                                  i.Status == IncidentStatus.Resolved ? 3 : 4)
                    .ThenBy(i => i.Id),

                _ => query
                    .OrderByDescending(i => i.CreatedAtUtc)
                    .ThenBy(i => i.Id)
            };

            var rows = await sortedQuery.Take(50).ToListAsync(ct);

            var result = rows.Select(row => new
            {
                row.Id,
                row.Title,
                row.Description,
                Severity = row.Severity.ToString(),
                Status = row.Status.ToString(),
                row.CreatedAtUtc
            });

            return Results.Ok(result);
        });
    }
}

// DTO структури
public sealed record CreateIncidentRequest(
    string? Title,
    string? Description,
    string? Severity,
    DateTimeOffset? OccurredAtUtc);

public sealed record CreatedIncidentResponse(
    Guid Id,
    string Title,
    string Description,
    string Severity,
    string Status,
    DateTimeOffset OccurredAtUtc,
    DateTimeOffset CreatedAtUtc);