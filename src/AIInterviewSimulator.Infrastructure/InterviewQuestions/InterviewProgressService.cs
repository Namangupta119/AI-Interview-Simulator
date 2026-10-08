using AIInterviewSimulator.Application.Common.Interfaces;
using AIInterviewSimulator.Application.Common.Models;
using AIInterviewSimulator.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AIInterviewSimulator.Infrastructure.InterviewQuestions;

public sealed class InterviewProgressService : IInterviewProgressService
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public InterviewProgressService(
        IAppDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<InterviewProgressResponse> GetProgressAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated ||
            !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "User must be authenticated to view interview progress.");
        }

        var userId = _currentUserService.UserId.Value;

        var session = await _dbContext.InterviewSessions
            .Include(s => s.Questions)
            .ThenInclude(q => q.Answer)
            .FirstOrDefaultAsync(
                s => s.Id == sessionId &&
                     s.UserId == userId &&
                     !s.IsDeleted,
                cancellationToken);

        if (session is null)
        {
            throw new KeyNotFoundException(
                "Interview session was not found.");
        }

        var questions = session.Questions
            .OrderBy(q => q.QuestionNumber)
            .ToList();

        var questionProgress = questions
            .Select(q => new InterviewQuestionProgress(
                QuestionNumber: q.QuestionNumber,
                Answered: q.Answer is not null &&
                          !string.IsNullOrWhiteSpace(q.Answer.AnswerText)
            ))
            .ToList();

        var answeredQuestions = questionProgress.Count(q => q.Answered);

        var currentQuestionNumber =
            questionProgress
                .FirstOrDefault(q => !q.Answered)?
                .QuestionNumber
            ?? questions.Count + 1; 

        return new InterviewProgressResponse(
            TotalQuestions: session.TotalQuestions,
            AnsweredQuestions: answeredQuestions,
            CurrentQuestionNumber: currentQuestionNumber,
            Questions: questionProgress
        );
    }
}