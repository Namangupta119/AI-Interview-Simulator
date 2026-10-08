using AIInterviewSimulator.Application.Common.Models;

namespace AIInterviewSimulator.Application.Common.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync(
        CancellationToken cancellationToken = default);
}