using AIInterviewSimulator.Application.Common.Models;

namespace AIInterviewSimulator.Application.Common.Interfaces;

/// <summary>
/// Defines authentication operations for the application.
/// Business logic is implemented in Infrastructure and consumed by API controllers.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">Registration data (email, username, full name, plain-text password).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="RegisterResponse"/> containing the new user's non-sensitive details.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if email or username is already taken.
    /// </exception>
    Task<RegisterResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates an existing user and returns an access token.
    /// </summary>
    /// <param name="request">Login identifier and password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication response containing the access token and safe user details.</returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the credentials are invalid or the user is inactive.
    /// </exception>
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
