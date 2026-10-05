using AIInterviewSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIInterviewSimulator.Infrastructure.Data.Configurations;

public class CandidateAnswerConfiguration : IEntityTypeConfiguration<CandidateAnswer>
{
    public void Configure(EntityTypeBuilder<CandidateAnswer> builder)
    {
        builder.ToTable("CandidateAnswers");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AnswerText)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(a => a.SubmittedAtUtc)
            .IsRequired();

        builder.HasOne(a => a.Question)
            .WithOne(q => q.Answer)
            .HasForeignKey<CandidateAnswer>(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Evaluation)
            .WithOne(e => e.Answer)
            .HasForeignKey<AnswerEvaluation>(e => e.AnswerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}