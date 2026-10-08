using AIInterviewSimulator.Domain.Enums;

namespace AIInterviewSimulator.Application.Common.Models;

public record CreateInterviewSessionRequest(
    string Role,
    ExperienceLevel ExperienceLevel,
    InterviewDifficulty Difficulty,
    int TotalQuestions,
    IReadOnlyCollection<InterviewTopic> Topics,
    IReadOnlyCollection<string>? CustomTopics
);

public record CreateInterviewSessionResponse(
    Guid SessionId,
    string Role,
    ExperienceLevel ExperienceLevel,
    InterviewDifficulty Difficulty,
    int TotalQuestions,
    IReadOnlyCollection<InterviewTopic> Topics,
    IReadOnlyCollection<string> CustomTopics,
    SessionStatus Status,
    DateTime StartedAtUtc
);

public record GenerateInterviewQuestionResponse(
    Guid QuestionId,
    int QuestionNumber,
    int TotalQuestions,
    string QuestionText,
    string ExpectedAnswerPoints,
    InterviewTopic? Topic,
    string? CustomTopic
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

public record InterviewHistoryItemResponse(
    Guid SessionId,
    string Role,
    ExperienceLevel ExperienceLevel,
    InterviewDifficulty Difficulty,
    int TotalQuestions,
    SessionStatus Status,
    decimal? OverallScore,
    DateTime StartedAtUtc,
    DateTime? CompletedAtUtc,
    IReadOnlyCollection<InterviewTopic> Topics,
    IReadOnlyCollection<string> CustomTopics,
    bool HasReport
);

public record InterviewHistoryResponse(
    IReadOnlyCollection<InterviewHistoryItemResponse> Items
);