using AIInterviewSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIInterviewSimulator.Infrastructure.Data.Configurations;

public class AnswerEvaluationConfiguration : IEntityTypeConfiguration<AnswerEvaluation>
{
    public void Configure(EntityTypeBuilder<AnswerEvaluation> builder)
    {
        builder.ToTable("AnswerEvaluations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Score)
            .IsRequired()
            .HasPrecision(4, 2);

        builder.Property(e => e.TechnicalCorrectness)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(e => e.Completeness)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(e => e.Strengths)
            .IsRequired()
            .HasMaxLength(3000);

        builder.Property(e => e.Weaknesses)
            .IsRequired()
            .HasMaxLength(3000);

        builder.Property(e => e.MissingConcepts)
            .IsRequired()
            .HasMaxLength(3000);

        builder.Property(e => e.ImprovementSuggestions)
            .IsRequired()
            .HasMaxLength(3000);

        builder.Property(e => e.IdealAnswer)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(e => e.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(e => e.AnswerId)
            .IsUnique();

        builder.HasOne(e => e.Answer)
            .WithOne(a => a.Evaluation)
            .HasForeignKey<AnswerEvaluation>(e => e.AnswerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}