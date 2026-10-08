using AIInterviewSimulator.Application.Common.Interfaces;
using AIInterviewSimulator.Application.Common.Models;
using AIInterviewSimulator.Domain.Entities;
using AIInterviewSimulator.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AIInterviewSimulator.Infrastructure.InterviewReports;

public sealed class InterviewReportService : IInterviewReportService
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAIInterviewService _aiInterviewService;

    public InterviewReportService(
        IAppDbContext dbContext,
        ICurrentUserService currentUserService,
        IAIInterviewService aiInterviewService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _aiInterviewService = aiInterviewService;
    }

    public async Task<InterviewReportResponse> GenerateAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated ||
            !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "User must be authenticated to generate the interview report.");
        }

        var userId = _currentUserService.UserId.Value;

        var session = await _dbContext.InterviewSessions
            .Include(s => s.Questions)
                .ThenInclude(q => q.Answer)
                    .ThenInclude(a => a!.Evaluation)
            .Include(s => s.Report)
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

        if (session.Status == SessionStatus.Completed)
        {
            throw new InvalidOperationException(
                "The interview report has already been generated.");
        }

        if (session.Status != SessionStatus.InProgress)
        {
            throw new InvalidOperationException(
                "A report can only be generated for an in-progress interview session.");
        }

        if (session.Questions.Count != session.TotalQuestions)
        {
            throw new InvalidOperationException(
                "The interview cannot be completed until all questions have been generated.");
        }

        var unansweredQuestions = session.Questions
            .Where(q => q.Answer is null || q.Answer.Evaluation is null)
            .ToList();

        if (unansweredQuestions.Count > 0)
        {
            throw new InvalidOperationException(
                "The interview cannot be completed until all questions have been answered and evaluated.");
        }

        var evaluatedAnswers = session.Questions
            .OrderBy(q => q.QuestionNumber)
            .Select(q => new EvaluatedQuestionAnswerSummary(
                QuestionText: q.QuestionText,
                CandidateAnswerText: q.Answer!.AnswerText,
                Score: (double)q.Answer.Evaluation!.Score,
                Feedback: q.Answer.Evaluation.TechnicalCorrectness,
                Topic: q.Topic.HasValue
                    ? q.Topic.Value.ToString()
                    : q.CustomTopic!
            ))
            .ToList();

        var summaryRequest = new GenerateSummaryRequest(
            TargetRole: session.Role,
            ExperienceLevel: session.ExperienceLevel,
            Answers: evaluatedAnswers
        );

        var summary = await _aiInterviewService.GenerateInterviewSummaryAsync(
            summaryRequest,
            cancellationToken);

        if (!summary.IsValid)
        {
            throw new InvalidOperationException(
                "The AI generated an invalid interview summary.");
        }

        if (summary.OverallScore < 0 || summary.OverallScore > 10)
        {
            throw new InvalidOperationException(
                "The AI generated an invalid overall score.");
        }

        if (summary.TopicScoresJson.Length > 5000)
        {
            throw new InvalidOperationException(
                "The AI generated topic scores exceed the allowed length.");
        }

        try
        {
            using var topicScoresDocument =
                JsonDocument.Parse(summary.TopicScoresJson);

            if (topicScoresDocument.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidOperationException(
                    "The AI generated topic scores must be a JSON object.");
            }
        }
        catch (JsonException)
        {
            throw new InvalidOperationException(
                "The AI generated invalid topic scores JSON.");
        }

        if (summary.StrengthSummary.Length > 4000)
        {
            throw new InvalidOperationException(
                "The AI generated strength summary exceeds the allowed length.");
        }

        if (summary.WeaknessSummary.Length > 3000)
        {
            throw new InvalidOperationException(
                "The AI generated weakness summary exceeds the allowed length.");
        }

        var recommendedTopicsText = string.Join(
            Environment.NewLine,
            summary.RecommendedTopics);

        if (recommendedTopicsText.Length > 4000)
        {
            throw new InvalidOperationException(
                "The AI generated recommended topics exceed the allowed length.");
        }

        if (summary.ImprovementPlan.Length > 5000)
        {
            throw new InvalidOperationException(
                "The AI generated improvement plan exceeds the allowed length.");
        }

        var report = new InterviewReport
        {
            Id = Guid.NewGuid(),
            SessionId = session.Id,
            OverallScore = (decimal)summary.OverallScore,
            StrengthSummary = summary.StrengthSummary.Trim(),
            WeaknessSummary = summary.WeaknessSummary.Trim(),
            RecommendedTopics = recommendedTopicsText,
            ImprovementPlan = summary.ImprovementPlan.Trim(),
            TopicScoresJson = summary.TopicScoresJson.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        session.Status = SessionStatus.Completed;
        session.OverallScore = report.OverallScore;
        session.CompletedAtUtc = report.CreatedAtUtc;

        _dbContext.InterviewReports.Add(report);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new InterviewReportResponse(
            ReportId: report.Id,
            SessionId: report.SessionId,
            OverallScore: report.OverallScore,
            StrengthSummary: report.StrengthSummary,
            WeaknessSummary: report.WeaknessSummary,
            RecommendedTopics: report.RecommendedTopics,
            ImprovementPlan: report.ImprovementPlan,
            TopicScoresJson: report.TopicScoresJson,
            CreatedAtUtc: report.CreatedAtUtc
        );
    }

    public async Task<InterviewReportResponse> GetAsync(
    Guid sessionId,
    CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated ||
            !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "User must be authenticated to view the interview report.");
        }

        var userId = _currentUserService.UserId.Value;

        var report = await _dbContext.InterviewReports
            .Include(r => r.Session)
            .FirstOrDefaultAsync(
                r => r.SessionId == sessionId &&
                    r.Session!.UserId == userId &&
                    !r.Session.IsDeleted,
                cancellationToken);

        if (report is null)
        {
            throw new KeyNotFoundException(
                "Interview report was not found.");
        }

        return new InterviewReportResponse(
            ReportId: report.Id,
            SessionId: report.SessionId,
            OverallScore: report.OverallScore,
            StrengthSummary: report.StrengthSummary,
            WeaknessSummary: report.WeaknessSummary,
            RecommendedTopics: report.RecommendedTopics,
            ImprovementPlan: report.ImprovementPlan,
            TopicScoresJson: report.TopicScoresJson,
            CreatedAtUtc: report.CreatedAtUtc
        );
    }
}