using AIInterviewSimulator.Application.Common.Interfaces;
using AIInterviewSimulator.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace AIInterviewSimulator.Infrastructure.AnswerEvaluations;

public sealed class AnswerEvaluationService : IAnswerEvaluationService
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAIInterviewService _aiInterviewService;

    public AnswerEvaluationService(
        IAppDbContext dbContext,
        ICurrentUserService currentUserService,
        IAIInterviewService aiInterviewService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _aiInterviewService = aiInterviewService;
    }

    public async Task<SubmitAnswerResponse> SubmitAsync(
        Guid questionId,
        string answerText,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated ||
            !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "User must be authenticated to submit an answer.");
        }

        if (string.IsNullOrWhiteSpace(answerText))
        {
            throw new ArgumentException(
                "Answer cannot be empty.",
                nameof(answerText));
        }

        answerText = answerText.Trim();

        if (answerText.Length > 5000)
        {
            throw new ArgumentException(
                "Answer cannot exceed 5000 characters.",
                nameof(answerText));
        }

        var userId = _currentUserService.UserId.Value;

        var question = await _dbContext.InterviewQuestions
            .Include(q => q.Session)
            .Include(q => q.Answer)
            .ThenInclude(a => a!.Evaluation)
            .FirstOrDefaultAsync(
                q => q.Id == questionId &&
                     q.Session != null &&
                     q.Session.UserId == userId &&
                     !q.Session.IsDeleted,
                cancellationToken);

        if (question is null)
        {
            throw new KeyNotFoundException(
                "Interview question was not found.");
        }

        if (question.Session!.Status != Domain.Enums.SessionStatus.InProgress)
        {
            throw new InvalidOperationException(
                "Answers can only be submitted for an in-progress interview session.");
        }

        if (question.Answer is not null)
        {
            throw new InvalidOperationException(
                "An answer has already been submitted for this question.");
        }

        var aiRequest = new EvaluateAnswerRequest(
            TargetRole: question.Session.Role,
            ExperienceLevel: question.Session.ExperienceLevel,
            QuestionText: question.QuestionText,
            ExpectedAnswerPoints: question.ExpectedAnswerPoints,
            CandidateAnswerText: answerText
        );

        var evaluation = await _aiInterviewService.EvaluateAnswerAsync(
            aiRequest,
            cancellationToken);

        if (!evaluation.IsValid)
        {
            throw new InvalidOperationException(
                "The AI generated an invalid answer evaluation.");
        }

        var answer = new Domain.Entities.CandidateAnswer
        {
            Id = Guid.NewGuid(),
            QuestionId = question.Id,
            AnswerText = answerText,
            SubmittedAtUtc = DateTime.UtcNow
        };

        var answerEvaluation = new Domain.Entities.AnswerEvaluation
        {
            Id = Guid.NewGuid(),
            AnswerId = answer.Id,
            Score = (decimal)evaluation.Score,
            TechnicalCorrectness = evaluation.TechnicalCorrectness,

            Completeness = evaluation.Completeness,

            Strengths = string.Join(
                Environment.NewLine,
                evaluation.Strengths),

            Weaknesses = string.Join(
                Environment.NewLine,
                evaluation.Weaknesses),

            MissingConcepts = string.Join(
                Environment.NewLine,
                evaluation.MissingConcepts),

            ImprovementSuggestions = string.Join(
                Environment.NewLine,
                evaluation.Improvements),

            IdealAnswer = evaluation.IdealAnswer,
            CreatedAtUtc = DateTime.UtcNow
        };

        answer.Evaluation = answerEvaluation;

        _dbContext.CandidateAnswers.Add(answer);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SubmitAnswerResponse(
            AnswerId: answer.Id,
            QuestionId: question.Id,
            Score: answerEvaluation.Score,
            TechnicalCorrectness: answerEvaluation.TechnicalCorrectness,
            Completeness: answerEvaluation.Completeness,
            Strengths: answerEvaluation.Strengths,
            Weaknesses: answerEvaluation.Weaknesses,
            MissingConcepts: answerEvaluation.MissingConcepts,
            ImprovementSuggestions: answerEvaluation.ImprovementSuggestions,
            IdealAnswer: answerEvaluation.IdealAnswer,
            SubmittedAtUtc: answer.SubmittedAtUtc
        );
    }
}