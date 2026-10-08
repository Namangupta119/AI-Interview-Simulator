using AIInterviewSimulator.Application.Common.Interfaces;
using AIInterviewSimulator.Application.Common.Models;
using AIInterviewSimulator.Domain.Entities;
using AIInterviewSimulator.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AIInterviewSimulator.Infrastructure.InterviewQuestions;

public sealed class InterviewQuestionService : IInterviewQuestionService
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAIInterviewService _aiInterviewService;

    public InterviewQuestionService(
        IAppDbContext dbContext,
        ICurrentUserService currentUserService,
        IAIInterviewService aiInterviewService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _aiInterviewService = aiInterviewService;
    }

    public async Task<GenerateInterviewQuestionResponse> GenerateNextQuestionAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated ||
            !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "User must be authenticated to generate interview questions.");
        }

        var userId = _currentUserService.UserId.Value;

        var session = await _dbContext.InterviewSessions
            .Include(s => s.SessionTopics)
            .Include(s => s.Questions)
            .ThenInclude(q => q.Answer)
            .ThenInclude(a => a!.Evaluation)
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

        if (session.Status != SessionStatus.InProgress)
        {
            throw new InvalidOperationException(
                "Questions can only be generated for an in-progress interview session.");
        }

        var existingQuestions = session.Questions
            .OrderBy(q => q.QuestionNumber)
            .ToList();

        var unansweredQuestion = existingQuestions
            .FirstOrDefault(q =>
                q.Answer is null ||
                string.IsNullOrWhiteSpace(q.Answer.AnswerText));

        if (unansweredQuestion is not null)
        {
            return new GenerateInterviewQuestionResponse(
                QuestionId: unansweredQuestion.Id,
                QuestionNumber: unansweredQuestion.QuestionNumber,
                TotalQuestions: session.TotalQuestions,
                QuestionText: unansweredQuestion.QuestionText,
                ExpectedAnswerPoints: unansweredQuestion.ExpectedAnswerPoints,
                Topic: unansweredQuestion.Topic,
                CustomTopic: unansweredQuestion.CustomTopic
            );
        }

        if (existingQuestions.Count >= session.TotalQuestions)
        {
            throw new InvalidOperationException(
                "All questions for this interview session have already been generated.");
        }

        if (session.SessionTopics.Count == 0)
        {
            throw new InvalidOperationException(
                "The interview session does not have any configured topics.");
        }

        var questionNumber = existingQuestions.Count + 1;

        var configuredTopics = session.SessionTopics
            .Select(st => new
            {
                Topic = st.Topic,
                CustomTopic = st.CustomTopic
            })
            .ToList();

            var nextConfiguredTopic = configuredTopics[
                (questionNumber - 1) % configuredTopics.Count
            ];

            var nextTopicName = nextConfiguredTopic.Topic.HasValue
                ? nextConfiguredTopic.Topic.Value.ToString()
                : nextConfiguredTopic.CustomTopic;

            if   (string.IsNullOrWhiteSpace(nextTopicName))
            {
                throw new InvalidOperationException(
                    "The configured interview topic is invalid.");
            }

        var previousQuestions = existingQuestions
            .Select(q => new PreviousQuestionContext(
                QuestionText: q.QuestionText,
                CandidateAnswerText: q.Answer?.AnswerText,
                Score: q.Answer?.Evaluation?.Score is decimal score
                    ? (double?)score
                    : null
            ))
            .ToList();

        var aiRequest = new GenerateQuestionRequest(
            TargetRole: session.Role,
            ExperienceLevel: session.ExperienceLevel,
            Difficulty: session.Difficulty,
            Topics: new List<string>
            {
                nextTopicName
            },  
            QuestionNumber: questionNumber,
            TotalQuestions: session.TotalQuestions,
            PreviousQuestions: previousQuestions
        );

        var generatedQuestion =
            await _aiInterviewService.GenerateQuestionAsync(
                aiRequest,
                cancellationToken);

        if (!generatedQuestion.IsValid)
        {
            throw new InvalidOperationException(
                "The AI generated an invalid interview question.");
        }

        if (!string.Equals(
            generatedQuestion.Topic.Trim(),
            nextTopicName,
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "The AI returned a topic different from the topic assigned to this question.");
        }

        var questionText = generatedQuestion.QuestionText.Trim();
        var expectedAnswerPoints =
            generatedQuestion.ExpectedAnswerPoints.Trim();

        if (questionText.Length > 2000)
        {
            throw new InvalidOperationException(
                "The AI generated question exceeds the allowed length.");
        }

        if (expectedAnswerPoints.Length > 3000)
        {
            throw new InvalidOperationException(
                "The AI generated expected answer points exceed the allowed length.");
        }

        var question = new InterviewQuestion
        {
            Id = Guid.NewGuid(),
            SessionId = session.Id,
            QuestionNumber = questionNumber,
            Topic = nextConfiguredTopic.Topic,
            CustomTopic = nextConfiguredTopic.CustomTopic,
            QuestionText = questionText,
            ExpectedAnswerPoints = expectedAnswerPoints,
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContext.InterviewQuestions.Add(question);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateInterviewQuestionResponse(
            QuestionId: question.Id,
            QuestionNumber: question.QuestionNumber,
            TotalQuestions: session.TotalQuestions,
            QuestionText: question.QuestionText,
            ExpectedAnswerPoints: question.ExpectedAnswerPoints,
            Topic: question.Topic,
            CustomTopic: question.CustomTopic
        );
    }
}