using AIInterviewSimulator.Application.Common.Models;

namespace AIInterviewSimulator.Application.Common.Interfaces;

public interface IInterviewReportService
{
    Task<InterviewReportResponse> GenerateAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);

        Task<InterviewReportResponse> GetAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);
}