namespace SecureLab.Api.Data.Entities;

public sealed class Incident
{
    public Guid Id { get; set; }
    public Guid OwnerUserId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public IncidentSeverity Severity { get; set; }
    public IncidentStatus Status { get; set; }
    public DateTimeOffset OccurredAtUtc { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
    public StudyUser Owner { get; set; } = null!;
    public List<IncidentComment> Comments { get; set; } = [];
    public List<IncidentStatusHistory> StatusHistory { get; set; } = [];
}

public enum IncidentSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public enum IncidentStatus
{
    New,
    Triaged,
    InProgress,
    Resolved,
    Closed
}
