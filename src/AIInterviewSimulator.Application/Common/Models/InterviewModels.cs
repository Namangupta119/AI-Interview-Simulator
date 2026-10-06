using AIInterviewSimulator.Domain.Enums;

namespace AIInterviewSimulator.Application.Common.Models;

public record CreateInterviewSessionRequest(
    string Role,
    ExperienceLevel ExperienceLevel,
    InterviewDifficulty Difficulty,
    int TotalQuestions,
    IReadOnlyCollection<InterviewTopic> Topics
);

public record CreateInterviewSessionResponse(
    Guid SessionId,
    string Role,
    ExperienceLevel ExperienceLevel,
    InterviewDifficulty Difficulty,
    int TotalQuestions,
    IReadOnlyCollection<InterviewTopic> Topics,
    SessionStatus Status,
    DateTime StartedAtUtc
);

public record GenerateInterviewQuestionResponse(
    Guid QuestionId,
    int QuestionNumber,
    int TotalQuestions,
    string QuestionText,
    string ExpectedAnswerPoints,
    InterviewTopic Topic
);

public record SubmitAnswerResponse(
    Guid AnswerId,
    Guid QuestionId,
    decimal Score,
    string TechnicalCorrectness,
    string Completeness,
    string Strengths,
    string Weaknesses,
    string MissingConcepts,
    string ImprovementSuggestions,
    string IdealAnswer,
    DateTime SubmittedAtUtc
);

public record InterviewReportResponse(
    Guid ReportId,
    Guid SessionId,
    decimal OverallScore,
    string StrengthSummary,
    string WeaknessSummary,
    string RecommendedTopics,
    string ImprovementPlan,
    string TopicScoresJson,
    DateTime CreatedAtUtc
);