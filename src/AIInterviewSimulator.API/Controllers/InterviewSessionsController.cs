using AIInterviewSimulator.Application.Common.Interfaces;
using AIInterviewSimulator.Application.Common.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIInterviewSimulator.API.Controllers;

[ApiController]
[Route("api/interviews/sessions")]
[Authorize]
public sealed class InterviewSessionsController : ControllerBase
{
    private readonly IInterviewSessionService _sessionService;
    private readonly IValidator<CreateInterviewSessionRequest> _validator;
    private readonly ILogger<InterviewSessionsController> _logger;
    private readonly IInterviewQuestionService _questionService;
    private readonly IInterviewProgressService _interviewProgressService;

    public InterviewSessionsController(
        IInterviewSessionService sessionService,
        IValidator<CreateInterviewSessionRequest> validator,
        ILogger<InterviewSessionsController> logger,
        IInterviewQuestionService questionService,
        IInterviewProgressService interviewProgressService)
    {
        _sessionService = sessionService;
        _validator = validator;
        _logger = logger;
        _questionService = questionService;
        _interviewProgressService = interviewProgressService;
    }

    /// <summary>
    /// Creates a new interview session for the authenticated candidate.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(
        typeof(CreateInterviewSessionResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        [FromBody] CreateInterviewSessionRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(error => error.ErrorMessage)
                        .ToArray());

            return ValidationProblem(
                new ValidationProblemDetails(errors));
        }

        try
        {
            var response = await _sessionService.CreateAsync(
                request,
                cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("{sessionId:guid}/questions/next")]
    [ProducesResponseType(
        typeof(GenerateInterviewQuestionResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GenerateNextQuestion(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _questionService.GenerateNextQuestionAsync(
                sessionId,
                cancellationToken);

            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                ex,
                "Unable to generate next question for session {SessionId}.",
                sessionId);

            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("{sessionId:guid}/progress")]
    public async Task<IActionResult> GetProgress(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _interviewProgressService.GetProgressAsync(
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
    }

    [HttpGet("history")]
    [ProducesResponseType(
        typeof(InterviewHistoryResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetHistory(
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _sessionService.GetHistoryAsync(
                cancellationToken);

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("{sessionId:guid}/abandon")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Abandon(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _sessionService.AbandonAsync(
                sessionId,
                cancellationToken);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}