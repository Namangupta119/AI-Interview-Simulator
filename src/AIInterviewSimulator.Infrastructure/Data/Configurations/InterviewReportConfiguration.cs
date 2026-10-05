using AIInterviewSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIInterviewSimulator.Infrastructure.Data.Configurations;

public class InterviewReportConfiguration : IEntityTypeConfiguration<InterviewReport>
{
    public void Configure(EntityTypeBuilder<InterviewReport> builder)
    {
        builder.ToTable("InterviewReports");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.OverallScore)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(r => r.StrengthSummary)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(r => r.WeaknessSummary)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(r => r.RecommendedTopics)
            .IsRequired()
            .HasMaxLength(3000);

        builder.Property(r => r.ImprovementPlan)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(r => r.TopicScoresJson)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(r => r.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(r => r.SessionId)
            .IsUnique();

        builder.HasOne(r => r.Session)
            .WithOne(s => s.Report)
            .HasForeignKey<InterviewReport>(r => r.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
