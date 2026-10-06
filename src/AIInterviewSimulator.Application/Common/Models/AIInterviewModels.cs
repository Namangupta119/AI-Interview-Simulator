using AIInterviewSimulator.Domain.Enums;

namespace AIInterviewSimulator.Application.Common.Models;

public record GenerateQuestionRequest(
    string TargetRole,
    ExperienceLevel ExperienceLevel,
    InterviewDifficulty Difficulty,
    IReadOnlyList<string> Topics,
    int QuestionNumber,
    int TotalQuestions,
    IReadOnlyList<PreviousQuestionContext>? PreviousQuestions = null
);

public record PreviousQuestionContext(
    string QuestionText,
    string? CandidateAnswerText,
    double? Score
);

public record GeneratedQuestionResult(
    string QuestionText,
    string ExpectedAnswerPoints,
    string Topic
)
{
    public bool IsValid =>
        !string.IsNullOrWhiteSpace(QuestionText) &&
        !string.IsNullOrWhiteSpace(ExpectedAnswerPoints) &&
        !string.IsNullOrWhiteSpace(Topic);
}

public record EvaluateAnswerRequest(
    string TargetRole,
    ExperienceLevel ExperienceLevel,
    string QuestionText,
    string ExpectedAnswerPoints,
    string CandidateAnswerText
);

public record EvaluatedAnswerResult(
    double Score,
    string TechnicalCorrectness,
    string Completeness,
    IReadOnlyList<string> Strengths,
    IReadOnlyList<string> Weaknesses,
    IReadOnlyList<string> MissingConcepts,
    IReadOnlyList<string> Improvements,
    string IdealAnswer
)
{
    public bool IsValid =>
        Score >= 0 &&
        Score <= 10 &&
        !string.IsNullOrWhiteSpace(TechnicalCorrectness) &&
        !string.IsNullOrWhiteSpace(Completeness) &&
        !string.IsNullOrWhiteSpace(IdealAnswer);
}

public record GenerateSummaryRequest(
    string TargetRole,
    ExperienceLevel ExperienceLevel,
    IReadOnlyList<EvaluatedQuestionAnswerSummary> Answers
);

public record EvaluatedQuestionAnswerSummary(
    string QuestionText,
    string CandidateAnswerText,
    double Score,
    string Feedback
);

public record InterviewSummaryResult(
    double OverallScore,
    string SummaryFeedback,
    IReadOnlyList<string> KeyStrengths,
    IReadOnlyList<string> AreasForImprovement
)
{
    public bool IsValid =>
        OverallScore >= 0 && OverallScore <= 10 &&
        !string.IsNullOrWhiteSpace(SummaryFeedback);
}
