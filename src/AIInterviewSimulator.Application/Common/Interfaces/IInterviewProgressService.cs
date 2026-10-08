using AIInterviewSimulator.Application.Common.Models;

namespace AIInterviewSimulator.Application.Common.Interfaces;

public interface IInterviewProgressService
{
    Task<InterviewProgressResponse> GetProgressAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);
}