using System.Security.Claims;
using AIInterviewSimulator.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AIInterviewSimulator.Infrastructure.Authentication;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var userId)
                ? userId
                : null;
        }
    }

    public string? Email =>
        User?.FindFirstValue(ClaimTypes.Email);

    public string? Username =>
        User?.FindFirstValue(ClaimTypes.Name);

    public IReadOnlyCollection<string> Roles =>
        User?
            .FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()
        ?? Array.Empty<string>();

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;
}