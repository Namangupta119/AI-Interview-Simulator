using AIInterviewSimulator.Application.Common.Interfaces;
using AIInterviewSimulator.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace AIInterviewSimulator.Infrastructure.Dashboard;

public sealed class DashboardService : IDashboardService
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public DashboardService(
        IAppDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<DashboardResponse> GetDashboardAsync(
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated ||
            !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "User must be authenticated to view the dashboard.");
        }

        var userId = _currentUserService.UserId.Value;

        var sessions = await _dbContext.InterviewSessions
            .Where(session =>
                session.UserId == userId &&
                !session.IsDeleted)
            .Include(session => session.Report)
            .OrderByDescending(session => session.StartedAtUtc)
            .ToListAsync(cancellationToken);

        var completedSessions = sessions
            .Where(session => session.Status == Domain.Enums.SessionStatus.Completed)
            .ToList();

        decimal? averageScore = completedSessions.Count > 0
            ? Math.Round(
                completedSessions
                    .Where(session => session.OverallScore.HasValue)
                    .Select(session => session.OverallScore!.Value)
                    .DefaultIfEmpty()
                    .Average(),
                1)
            : null;

        var recentInterviews = sessions
            .Take(5)
            .Select(session => new DashboardRecentInterviewResponse(
                SessionId: session.Id,
                Role: session.Role,
                OverallScore: session.OverallScore,
                Status: session.Status,
                StartedAtUtc: session.StartedAtUtc,
                HasReport: session.Report is not null
            ))
            .ToArray();

        return new DashboardResponse(
            TotalInterviews: sessions.Count,
            CompletedInterviews: completedSessions.Count,
            AverageScore: averageScore,
            RecentInterviews: recentInterviews
        );
    }
}