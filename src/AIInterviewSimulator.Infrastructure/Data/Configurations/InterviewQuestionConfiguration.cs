using AIInterviewSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIInterviewSimulator.Infrastructure.Persistence.Configurations;

public class InterviewQuestionConfiguration
    : IEntityTypeConfiguration<InterviewQuestion>
{
    public void Configure(EntityTypeBuilder<InterviewQuestion> builder)
    {
        builder.ToTable("InterviewQuestions");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.QuestionNumber)
            .IsRequired();

        builder.Property(q => q.Topic)
            .IsRequired(false);

        builder.Property(q => q.CustomTopic)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(q => q.QuestionText)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(q => q.ExpectedAnswerPoints)
            .IsRequired()
            .HasMaxLength(3000);

        builder.Property(q => q.CreatedAtUtc)
            .IsRequired();

        builder.HasOne(q => q.Session)
            .WithMany(s => s.Questions)
            .HasForeignKey(q => q.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(q => q.Answer)
            .WithOne(a => a.Question)
            .HasForeignKey<CandidateAnswer>(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}