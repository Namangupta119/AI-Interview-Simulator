namespace AIInterviewSimulator.Application.Common.Models;

/// <summary>
/// Inbound DTO for user registration. Validated by RegisterRequestValidator.
/// </summary>
/// <param name="Email">Unique email address for the account.</param>
/// <param name="Username">Unique display username (3–50 characters, alphanumeric + underscore).</param>
/// <param name="FullName">User's full name (2–150 characters).</param>
/// <param name="Password">Plain-text password (min 8 chars, mixed case, digit required).
/// Never stored; hashed immediately inside IAuthService.</param>
public record RegisterRequest(
    string Email,
    string Username,
    string FullName,
    string Password
);

/// <summary>
/// Outbound DTO returned after successful registration.
/// Deliberately excludes PasswordHash and any sensitive fields.
/// </summary>
/// <param name="UserId">Newly created user's identifier.</param>
/// <param name="Email">Registered email address.</param>
/// <param name="Username">Registered username.</param>
/// <param name="FullName">User's full name.</param>
public record RegisterResponse(
    Guid UserId,
    string Email,
    string Username,
    string FullName
);


/// <summary>
/// Inbound DTO for user login.
/// The identifier can be either a registered email address or username.
/// </summary>
public record LoginRequest(
    string Identifier,
    string Password
);

/// <summary>
/// Authentication response returned after successful login.
/// </summary>
public record LoginResponse(
    Guid UserId,
    string Email,
    string Username,
    IReadOnlyCollection<string> Roles,
    string AccessToken,
    DateTime ExpiresAtUtc
);