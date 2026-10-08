using AIInterviewSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIInterviewSimulator.Infrastructure.Persistence.Configurations;

public class SessionTopicConfiguration : IEntityTypeConfiguration<SessionTopic>
{
    public void Configure(EntityTypeBuilder<SessionTopic> builder)
    {
        builder.ToTable("SessionTopics");

        builder.HasKey(st => st.Id);

        builder.Property(st => st.Topic)
            .IsRequired(false);

        builder.Property(st => st.CustomTopic)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.HasOne(st => st.Session)
            .WithMany(s => s.SessionTopics)
            .HasForeignKey(st => st.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(st => new { st.SessionId, st.Topic })
            .IsUnique()
            .HasFilter("[Topic] IS NOT NULL");

        builder.HasIndex(st => new { st.SessionId, st.CustomTopic })
            .IsUnique()
            .HasFilter("[CustomTopic] IS NOT NULL");
    }
}