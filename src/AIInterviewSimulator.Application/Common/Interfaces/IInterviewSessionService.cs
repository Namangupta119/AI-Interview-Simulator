using AIInterviewSimulator.Application.Common.Models;

namespace AIInterviewSimulator.Application.Common.Interfaces;

public interface IInterviewSessionService
{
    Task<CreateInterviewSessionResponse> CreateAsync(
        CreateInterviewSessionRequest request,
        CancellationToken cancellationToken = default);

    Task<InterviewHistoryResponse> GetHistoryAsync(
        CancellationToken cancellationToken = default);

    Task AbandonAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);
}