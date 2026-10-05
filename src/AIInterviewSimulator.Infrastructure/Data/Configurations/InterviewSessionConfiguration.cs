using AIInterviewSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIInterviewSimulator.Infrastructure.Data.Configurations;

public class InterviewSessionConfiguration : IEntityTypeConfiguration<InterviewSession>
{
    public void Configure(EntityTypeBuilder<InterviewSession> builder)
    {
        builder.ToTable("InterviewSessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Role)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.ExperienceLevel)
            .IsRequired();

        builder.Property(s => s.Difficulty)
            .IsRequired();

        builder.Property(s => s.TotalQuestions)
            .IsRequired();

        builder.Property(s => s.Status)
            .IsRequired();

        builder.Property(s => s.OverallScore)
            .HasPrecision(5, 2);

        builder.Property(s => s.StartedAtUtc)
            .IsRequired();

        builder.Property(s => s.CompletedAtUtc);

        builder.Property(s => s.CreatedAtUtc)
            .IsRequired();

        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.Status);

        builder.HasOne(s => s.User)
            .WithMany(u => u.InterviewSessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.SessionTopics)
            .WithOne(st => st.Session)
            .HasForeignKey(st => st.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Questions)
            .WithOne(q => q.Session)
            .HasForeignKey(q => q.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Report)
            .WithOne(r => r.Session)
            .HasForeignKey<InterviewReport>(r => r.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
