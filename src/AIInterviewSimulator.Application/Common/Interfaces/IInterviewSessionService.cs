using AIInterviewSimulator.Application.Common.Models;

namespace AIInterviewSimulator.Application.Common.Interfaces;

public interface IInterviewSessionService
{
    Task<CreateInterviewSessionResponse> CreateAsync(
        CreateInterviewSessionRequest request,
        CancellationToken cancellationToken = default);
}