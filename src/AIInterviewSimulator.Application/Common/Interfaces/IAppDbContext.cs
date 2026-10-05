using AIInterviewSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIInterviewSimulator.Application.Common.Interfaces;

/// <summary>
/// Project-specific database abstraction that prevents the Application layer
/// from directly depending on the concrete Infrastructure AppDbContext type.
/// Note: Exposes EF Core DbSet<T> types and is not intended to abstract away EF Core.
/// </summary>
public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<InterviewSession> InterviewSessions { get; }
    DbSet<SessionTopic> SessionTopics { get; }
    DbSet<InterviewQuestion> InterviewQuestions { get; }
    DbSet<CandidateAnswer> CandidateAnswers { get; }
    DbSet<AnswerEvaluation> AnswerEvaluations { get; }
    DbSet<InterviewReport> InterviewReports { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
