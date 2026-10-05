using AIInterviewSimulator.Application.Common.Interfaces;
using AIInterviewSimulator.Application.Common.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AIInterviewSimulator.API.Controllers;

/// <summary>
/// Handles authentication operations: registration, login (future), etc.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ICurrentUserService currentUserService,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _currentUserService = currentUserService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new candidate account.
    /// </summary>
    /// <param name="request">Registration payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>201 Created with the new user's non-sensitive details on success.</returns>
    /// <response code="201">User registered successfully.</response>
    /// <response code="400">Validation failed or email/username already taken.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        // 1. FluentValidation — structural rules (format, length, etc.)
        var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        // 2. Business logic (duplicate check, hash, persist) — inside IAuthService
        try
        {
            var response = await _authService.RegisterAsync(request, cancellationToken);

            return CreatedAtAction(
                actionName: null,
                value: response);
        }
        catch (InvalidOperationException ex)
        {
            // Duplicate email or username — return 400 with a clear message
            _logger.LogWarning("Registration rejected: {Reason}", ex.Message);

            return Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Registration Failed");
        }
    }

    /// <summary>
    /// Authenticates an existing user and returns a JWT access token.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>JWT access token and non-sensitive user details.</returns>
    /// <response code="200">Login successful.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">Invalid credentials or inactive account.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _loginValidator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            return ValidationProblem(
                new ValidationProblemDetails(errors));
        }

        try
        {
            var response = await _authService.LoginAsync(
                request,
                cancellationToken);

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning(
                "Login failed for identifier: {Identifier}",
                request.Identifier);

            return Unauthorized(new
            {
                message = "Invalid email/username or password."
            });
        }
    }

    /// <summary>
    /// Returns the currently authenticated user's information.
    /// </summary>
    /// <returns>Current user's non-sensitive identity information.</returns>
    /// <response code="200">Authenticated user information.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        if (!_currentUserService.IsAuthenticated ||
            !_currentUserService.UserId.HasValue)
        {
            return Unauthorized();
        }

        return Ok(new
        {
            userId = _currentUserService.UserId,
            email = _currentUserService.Email,
            username = _currentUserService.Username,
            roles = _currentUserService.Roles
        });
    }
}
