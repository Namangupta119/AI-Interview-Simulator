using AIInterviewSimulator.Application.Common.Models;

namespace AIInterviewSimulator.Application.Common.Interfaces;

public interface IInterviewQuestionService
{
    Task<GenerateInterviewQuestionResponse> GenerateNextQuestionAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);
}