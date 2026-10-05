using AIInterviewSimulator.Domain.Enums;

namespace AIInterviewSimulator.Domain.Entities;

public class InterviewSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string Role { get; set; } = string.Empty;
    public ExperienceLevel ExperienceLevel { get; set; } = ExperienceLevel.MidLevel;
    public InterviewDifficulty Difficulty { get; set; } = InterviewDifficulty.Medium;
    public int TotalQuestions { get; set; } = 5;

    public SessionStatus Status { get; set; } = SessionStatus.InProgress;
    public decimal? OverallScore { get; set; }

    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    public ICollection<SessionTopic> SessionTopics { get; set; } = new List<SessionTopic>();
    public ICollection<InterviewQuestion> Questions { get; set; } = new List<InterviewQuestion>();
    public InterviewReport? Report { get; set; }
}
