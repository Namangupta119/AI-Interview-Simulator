namespace AIInterviewSimulator.Domain.Entities;

public class CandidateAnswer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid QuestionId { get; set; }
    public InterviewQuestion? Question { get; set; }

    public string AnswerText { get; set; } = string.Empty;
    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;

    public AnswerEvaluation? Evaluation { get; set; }
}
