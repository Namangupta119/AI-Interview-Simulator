using AIInterviewSimulator.Application.Common.Interfaces;
using AIInterviewSimulator.Application.Common.Models;
using AIInterviewSimulator.Domain.Entities;
using AIInterviewSimulator.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AIInterviewSimulator.Infrastructure.InterviewSessions;

public sealed class InterviewSessionService : IInterviewSessionService
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public InterviewSessionService(
        IAppDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<CreateInterviewSessionResponse> CreateAsync(
        CreateInterviewSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated ||
            !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "User must be authenticated to create an interview session.");
        }

        var userId = _currentUserService.UserId.Value;

        var session = new InterviewSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Role = request.Role.Trim(),
            ExperienceLevel = request.ExperienceLevel,
            Difficulty = request.Difficulty,
            TotalQuestions = request.TotalQuestions,
            Status = SessionStatus.InProgress,
            StartedAtUtc = DateTime.UtcNow,
            CreatedAtUtc = DateTime.UtcNow,
            IsDeleted = false
        };

        foreach (var topic in request.Topics.Distinct())
        {
            session.SessionTopics.Add(new SessionTopic
            {
                SessionId = session.Id,
                Topic = topic,
                CustomTopic = null
            });
        }

        foreach (var customTopic in (request.CustomTopics ?? Array.Empty<string>())
             .Select(topic => topic.Trim())
             .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            session.SessionTopics.Add(new SessionTopic
            {
                SessionId = session.Id,
                Topic = null,
                CustomTopic = customTopic
            });
        }

        _dbContext.InterviewSessions.Add(session);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateInterviewSessionResponse(
            SessionId: session.Id,
            Role: session.Role,
            ExperienceLevel: session.ExperienceLevel,
            Difficulty: session.Difficulty,
            TotalQuestions: session.TotalQuestions,
            Topics: session.SessionTopics
                .Where(st => st.Topic.HasValue)
                .Select(st => st.Topic!.Value)
                .ToArray(),

            CustomTopics: session.SessionTopics
                .Where(st => !string.IsNullOrWhiteSpace(st.CustomTopic))
                .Select(st => st.CustomTopic!)
                .ToArray(),
            Status: session.Status,
            StartedAtUtc: session.StartedAtUtc
        );
    }

    public async Task<InterviewHistoryResponse> GetHistoryAsync(
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated ||
            !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "User must be authenticated to view interview history.");
        }

        var userId = _currentUserService.UserId.Value;

        var sessions = await _dbContext.InterviewSessions
            .Where(session =>
                session.UserId == userId &&
                !session.IsDeleted)
            .Include(session => session.SessionTopics)
            .Include(session => session.Report)
            .OrderByDescending(session => session.StartedAtUtc)
            .ToListAsync(cancellationToken);

        var items = sessions
            .Select(session => new InterviewHistoryItemResponse(
                SessionId: session.Id,
                Role: session.Role,
                ExperienceLevel: session.ExperienceLevel,
                Difficulty: session.Difficulty,
                TotalQuestions: session.TotalQuestions,
                Status: session.Status,
                OverallScore: session.OverallScore,
                StartedAtUtc: session.StartedAtUtc,
                CompletedAtUtc: session.CompletedAtUtc,
                Topics: session.SessionTopics
                    .Where(st => st.Topic.HasValue)
                    .Select(st => st.Topic!.Value)
                    .ToArray(),
                CustomTopics: session.SessionTopics
                    .Where(st => !string.IsNullOrWhiteSpace(st.CustomTopic))
                    .Select(st => st.CustomTopic!)
                    .ToArray(),
                HasReport: session.Report is not null
            ))
            .ToArray();

        return new InterviewHistoryResponse(items);
    }
}