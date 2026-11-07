namespace SecureLab.Api.Data.Entities;

public sealed class IncidentStatusHistory
{
    public Guid Id { get; set; }
    public Guid IncidentId { get; set; }
    public Guid ChangedByUserId { get; set; }
    public IncidentStatus OldStatus { get; set; }
    public IncidentStatus NewStatus { get; set; }
    public required string Note { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public Incident Incident { get; set; } = null!;
    public StudyUser ChangedByUser { get; set; } = null!;
}
