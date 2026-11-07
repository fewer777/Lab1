namespace SecureLab.Api.Data.Entities;

public sealed class IncidentComment
{
    public Guid Id { get; set; }
    public Guid IncidentId { get; set; }
    public Guid AuthorUserId { get; set; }
    public required string Text { get; set; }
    public bool IsInternal { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public Incident Incident { get; set; } = null!;
    public StudyUser Author { get; set; } = null!;
}
