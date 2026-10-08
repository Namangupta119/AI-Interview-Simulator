namespace AIInterviewSimulator.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? Email { get; }

    string? Username { get; }

    string? FullName { get; }

    IReadOnlyCollection<string> Roles { get; }

    bool IsAuthenticated { get; }
}