using AIInterviewSimulator.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIInterviewSimulator.API.Controllers;

[ApiController]
[Route("api/interviews/sessions")]
[Authorize]
public class InterviewReportsController : ControllerBase
{
    private readonly IInterviewReportService _interviewReportService;

    public InterviewReportsController(
        IInterviewReportService interviewReportService)
    {
        _interviewReportService = interviewReportService;
    }

    [HttpPost("{sessionId:guid}/complete")]
    public async Task<IActionResult> GenerateReport(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _interviewReportService.GenerateAsync(
                sessionId,
                cancellationToken);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }
}