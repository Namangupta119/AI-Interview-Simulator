using AIInterviewSimulator.Application.Common.Interfaces;
using AIInterviewSimulator.Application.Common.Models;
using AIInterviewSimulator.Domain.Entities;
using AIInterviewSimulator.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AIInterviewSimulator.Infrastructure.Authentication;

/// <summary>
/// Concrete implementation of <see cref="IAuthService"/>.
/// Coordinates user registration by: validating uniqueness, hashing the password,
/// persisting the user, and assigning the Candidate role.
/// Uses <see cref="IAppDbContext"/> directly — no generic repository.
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AuthService> _logger;
    private readonly ITokenService _tokenService;

    public AuthService(
        IAppDbContext db,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<RegisterResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Normalize inputs
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedUsername = request.Username.Trim();

        // 2. Check for duplicate email
        var emailExists = await _db.Users
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            _logger.LogWarning("Registration attempt with already-registered email: {Email}", normalizedEmail);
            throw new InvalidOperationException($"Email '{request.Email}' is already registered.");
        }

        // 3. Check for duplicate username
        var usernameExists = await _db.Users
            .AnyAsync(u => u.Username == normalizedUsername, cancellationToken);

        if (usernameExists)
        {
            _logger.LogWarning("Registration attempt with already-taken username: {Username}", normalizedUsername);
            throw new InvalidOperationException($"Username '{request.Username}' is already taken.");
        }

        // 4. Hash the password — plain text is never persisted
        var passwordHash = _passwordHasher.Hash(request.Password);

        // 5. Build the User entity
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            Username = normalizedUsername,
            FullName = request.FullName.Trim(),
            PasswordHash = passwordHash,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        // 6. Assign the Candidate role (Id = 2, per RoleConfiguration seed + UserRoleType enum)
        var candidateRole = new UserRole
        {
            UserId = user.Id,
            RoleId = (int)UserRoleType.Candidate   // resolves to 2 — matches seeded Roles row
        };

        // 7. Persist atomically
        _db.Users.Add(user);
        _db.UserRoles.Add(candidateRole);

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "New user registered. UserId={UserId}, Email={Email}, Username={Username}",
            user.Id, user.Email, user.Username);

        // 8. Return safe response — PasswordHash is intentionally excluded
        return new RegisterResponse(
            UserId: user.Id,
            Email: user.Email,
            Username: user.Username,
            FullName: user.FullName
        );
    }

    public async Task<LoginResponse> LoginAsync(
    LoginRequest request,
    CancellationToken cancellationToken = default)
{
    var identifier = request.Identifier.Trim();

    var user = await _db.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .SingleOrDefaultAsync(
            u => u.Email == identifier.ToLowerInvariant()
                 || u.Username == identifier,
            cancellationToken);

    if (user is null || !user.IsActive)
    {
        throw new UnauthorizedAccessException(
            "Invalid email/username or password.");
    }

    var passwordValid = _passwordHasher.Verify(
        request.Password,
        user.PasswordHash);

    if (!passwordValid)
    {
        throw new UnauthorizedAccessException(
            "Invalid email/username or password.");
    }

    var roles = user.UserRoles
        .Where(ur => ur.Role != null)
        .Select(ur => ur.Role!.Name)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

    var tokenResult = await _tokenService.GenerateAccessTokenAsync(user.Id);

    _logger.LogInformation(
        "User logged in successfully. UserId={UserId}",
        user.Id);

    return new LoginResponse(
        UserId: user.Id,
        Email: user.Email,
        Username: user.Username,
        Roles: roles,
        AccessToken: tokenResult.AccessToken,
        ExpiresAtUtc: tokenResult.ExpiresAtUtc);
}
}
