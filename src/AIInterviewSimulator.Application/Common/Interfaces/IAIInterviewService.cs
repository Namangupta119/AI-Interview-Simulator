using AIInterviewSimulator.Application.Common.Models;

namespace AIInterviewSimulator.Application.Common.Interfaces;

public interface IAIInterviewService
{
    Task<GeneratedQuestionResult> GenerateQuestionAsync(
        GenerateQuestionRequest request,
        CancellationToken cancellationToken = default);

    Task<EvaluatedAnswerResult> EvaluateAnswerAsync(
        EvaluateAnswerRequest request,
        CancellationToken cancellationToken = default);

    Task<InterviewSummaryResult> GenerateInterviewSummaryAsync(
        GenerateSummaryRequest request,
        CancellationToken cancellationToken = default);
}
