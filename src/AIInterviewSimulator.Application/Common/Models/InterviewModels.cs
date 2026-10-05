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