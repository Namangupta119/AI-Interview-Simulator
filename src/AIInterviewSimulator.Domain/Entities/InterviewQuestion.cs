using AIInterviewSimulator.Domain.Enums;

namespace AIInterviewSimulator.Domain.Entities;

public class InterviewQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SessionId { get; set; }

    public InterviewSession? Session { get; set; }

    public int QuestionNumber { get; set; }

    public InterviewTopic? Topic { get; set; }

    public string? CustomTopic { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public string ExpectedAnswerPoints { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public CandidateAnswer? Answer { get; set; }
}