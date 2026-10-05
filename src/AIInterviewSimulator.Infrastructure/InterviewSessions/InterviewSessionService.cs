using AIInterviewSimulator.Application.Common.Interfaces;
using AIInterviewSimulator.Application.Common.Models;
using AIInterviewSimulator.Domain.Entities;
using AIInterviewSimulator.Domain.Enums;

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
                Topic = topic
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
                .Select(st => st.Topic)
                .ToArray(),
            Status: session.Status,
            StartedAtUtc: session.StartedAtUtc
        );
    }
}