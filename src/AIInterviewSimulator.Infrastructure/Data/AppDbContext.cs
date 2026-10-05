using AIInterviewSimulator.Application.Common.Interfaces;
using AIInterviewSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIInterviewSimulator.Infrastructure.Data;

/// <summary>
/// Concrete EF Core database context implementing the Application-level IAppDbContext abstraction.
/// </summary>
public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<InterviewSession> InterviewSessions => Set<InterviewSession>();
    public DbSet<SessionTopic> SessionTopics => Set<SessionTopic>();
    public DbSet<InterviewQuestion> InterviewQuestions => Set<InterviewQuestion>();
    public DbSet<CandidateAnswer> CandidateAnswers => Set<CandidateAnswer>();
    public DbSet<AnswerEvaluation> AnswerEvaluations => Set<AnswerEvaluation>();
    public DbSet<InterviewReport> InterviewReports => Set<InterviewReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
