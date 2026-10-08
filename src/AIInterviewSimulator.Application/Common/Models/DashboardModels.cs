using AIInterviewSimulator.Domain.Enums;

namespace AIInterviewSimulator.Application.Common.Models;

public record DashboardRecentInterviewResponse(
    Guid SessionId,
    string Role,
    decimal? OverallScore,
    SessionStatus Status,
    DateTime StartedAtUtc,
    bool HasReport
);

public record DashboardResponse(
    int TotalInterviews,
    int CompletedInterviews,
    decimal? AverageScore,
    IReadOnlyCollection<DashboardRecentInterviewResponse> RecentInterviews
);