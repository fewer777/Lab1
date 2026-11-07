using Microsoft.EntityFrameworkCore;
using SecureLab.Api.Data.Entities;

namespace SecureLab.Api.Data;

public sealed class SecureLabDbContext(DbContextOptions<SecureLabDbContext> options)
    : DbContext(options)
{
    public DbSet<StudyUser> Users => Set<StudyUser>();
    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<IncidentComment> IncidentComments => Set<IncidentComment>();
    public DbSet<IncidentStatusHistory> IncidentStatusHistory => Set<IncidentStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<StudyUser>();
        user.ToTable("study_users");
        user.HasKey(item => item.Id);
        user.Property(item => item.Id).HasColumnName("id");
        user.Property(item => item.UserName).HasColumnName("user_name").HasMaxLength(64);
        user.Property(item => item.DisplayName).HasColumnName("display_name").HasMaxLength(120);
        user.Property(item => item.Email).HasColumnName("email").HasMaxLength(254);
        user.Property(item => item.Role).HasColumnName("role").HasMaxLength(32);
        user.HasIndex(item => item.UserName).IsUnique();
        user.HasIndex(item => item.Email).IsUnique();

        var incident = modelBuilder.Entity<Incident>();
        incident.ToTable("incidents");
        incident.HasKey(item => item.Id);
        incident.Property(item => item.Id).HasColumnName("id");
        incident.Property(item => item.OwnerUserId).HasColumnName("owner_user_id");
        incident.Property(item => item.Title).HasColumnName("title").HasMaxLength(160);
        incident.Property(item => item.Description).HasColumnName("description").HasMaxLength(4000);
        incident.Property(item => item.Severity).HasColumnName("severity").HasConversion<string>().HasMaxLength(16);
        incident.Property(item => item.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(24);
        incident.Property(item => item.OccurredAtUtc).HasColumnName("occurred_at_utc");
        incident.Property(item => item.CreatedAtUtc).HasColumnName("created_at_utc");
        incident.Property(item => item.UpdatedAtUtc).HasColumnName("updated_at_utc");
        incident.HasIndex(item => item.OwnerUserId);
        incident.HasIndex(item => item.Status);
        incident.HasIndex(item => item.Severity);
        incident.HasOne(item => item.Owner)
            .WithMany(item => item.Incidents)
            .HasForeignKey(item => item.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        var comment = modelBuilder.Entity<IncidentComment>();
        comment.ToTable("incident_comments");
        comment.HasKey(item => item.Id);
        comment.Property(item => item.Id).HasColumnName("id");
        comment.Property(item => item.IncidentId).HasColumnName("incident_id");
        comment.Property(item => item.AuthorUserId).HasColumnName("author_user_id");
        comment.Property(item => item.Text).HasColumnName("text").HasMaxLength(2000);
        comment.Property(item => item.IsInternal).HasColumnName("is_internal");
        comment.Property(item => item.CreatedAtUtc).HasColumnName("created_at_utc");
        comment.HasOne(item => item.Incident)
            .WithMany(item => item.Comments)
            .HasForeignKey(item => item.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);
        comment.HasOne(item => item.Author)
            .WithMany()
            .HasForeignKey(item => item.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        var history = modelBuilder.Entity<IncidentStatusHistory>();
        history.ToTable("incident_status_history");
        history.HasKey(item => item.Id);
        history.Property(item => item.Id).HasColumnName("id");
        history.Property(item => item.IncidentId).HasColumnName("incident_id");
        history.Property(item => item.ChangedByUserId).HasColumnName("changed_by_user_id");
        history.Property(item => item.OldStatus).HasColumnName("old_status").HasConversion<string>().HasMaxLength(24);
        history.Property(item => item.NewStatus).HasColumnName("new_status").HasConversion<string>().HasMaxLength(24);
        history.Property(item => item.Note).HasColumnName("note").HasMaxLength(1000);
        history.Property(item => item.CreatedAtUtc).HasColumnName("created_at_utc");
        history.HasOne(item => item.Incident)
            .WithMany(item => item.StatusHistory)
            .HasForeignKey(item => item.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);
        history.HasOne(item => item.ChangedByUser)
            .WithMany()
            .HasForeignKey(item => item.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
