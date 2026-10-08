namespace AIInterviewSimulator.Application.Common.Models;

public record InterviewQuestionProgress(
    int QuestionNumber,
    bool Answered
);

public record InterviewProgressResponse(
    int TotalQuestions,
    int AnsweredQuestions,
    int CurrentQuestionNumber,
    IReadOnlyCollection<InterviewQuestionProgress> Questions
);