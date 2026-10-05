namespace AIInterviewSimulator.Domain.Entities;

public class InterviewReport
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SessionId { get; set; }
    public InterviewSession? Session { get; set; }

    public decimal OverallScore { get; set; }
    public string StrengthSummary { get; set; } = string.Empty;
    public string WeaknessSummary { get; set; } = string.Empty;
    public string RecommendedTopics { get; set; } = string.Empty;
    public string ImprovementPlan { get; set; } = string.Empty;
    public string TopicScoresJson { get; set; } = "{}";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
