using AIInterviewSimulator.Application.Common.Models;

namespace AIInterviewSimulator.Application.Common.Interfaces;

public interface IAnswerEvaluationService
{
    Task<SubmitAnswerResponse> SubmitAsync(
        Guid questionId,
        string answerText,
        CancellationToken cancellationToken = default);
}