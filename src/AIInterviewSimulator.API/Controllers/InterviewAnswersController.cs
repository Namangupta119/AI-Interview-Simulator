using AIInterviewSimulator.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIInterviewSimulator.API.Controllers;

[ApiController]
[Route("api/interviews/questions")]
[Authorize]
public class InterviewAnswersController : ControllerBase
{
    private readonly IAnswerEvaluationService _answerEvaluationService;

    public InterviewAnswersController(
        IAnswerEvaluationService answerEvaluationService)
    {
        _answerEvaluationService = answerEvaluationService;
    }

    [HttpPost("{questionId:guid}/answer")]
    public async Task<IActionResult> SubmitAnswer(
        Guid questionId,
        [FromBody] SubmitAnswerRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _answerEvaluationService.SubmitAsync(
                questionId,
                request.AnswerText,
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
        catch (ArgumentException ex)
        {
            return BadRequest(new
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

public record SubmitAnswerRequest(string AnswerText);