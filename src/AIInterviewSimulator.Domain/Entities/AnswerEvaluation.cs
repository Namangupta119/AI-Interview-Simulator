namespace AIInterviewSimulator.Domain.Entities;

public class AnswerEvaluation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AnswerId { get; set; }
    public CandidateAnswer? Answer { get; set; }

    public decimal Score { get; set; }
    public string TechnicalCorrectness { get; set; } = string.Empty;
    public string Completeness { get; set; } = string.Empty;
    public string Strengths { get; set; } = string.Empty;
    public string Weaknesses { get; set; } = string.Empty;
    public string MissingConcepts { get; set; } = string.Empty;
    public string ImprovementSuggestions { get; set; } = string.Empty;
    public string IdealAnswer { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
