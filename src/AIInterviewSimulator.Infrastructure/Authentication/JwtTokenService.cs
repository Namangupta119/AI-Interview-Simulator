using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AIInterviewSimulator.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AIInterviewSimulator.Infrastructure.Authentication;

public sealed class JwtTokenService : ITokenService
{
    private readonly IAppDbContext _dbContext;
    private readonly JwtSettings _settings;

    public JwtTokenService(
        IAppDbContext dbContext,
        IOptions<JwtSettings> options)
    {
        _dbContext = dbContext;
        _settings = options.Value;
    }

    public async Task<TokenResult> GenerateAccessTokenAsync(Guid userId)
    {
        if (string.IsNullOrWhiteSpace(_settings.SecretKey))
        {
            throw new InvalidOperationException(
                "JWT signing key is not configured.");
        }

        if (_settings.SecretKey.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT signing key must be at least 32 characters long.");
        }

        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .SingleOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            throw new InvalidOperationException(
                "User could not be found.");
        }

        var roles = user.UserRoles
            .Where(ur => ur.Role != null)
            .Select(ur => ur.Role!.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new("full_name", user.FullName)
        };

        claims.AddRange(
            roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(
            _settings.AccessTokenExpiryMinutes);

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.SecretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return new TokenResult(
            accessToken,
            expiresAtUtc);
    }
}