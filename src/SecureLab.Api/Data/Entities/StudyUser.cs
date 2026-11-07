namespace SecureLab.Api.Data.Entities;

public sealed class StudyUser
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
    public List<Incident> Incidents { get; set; } = [];
}
