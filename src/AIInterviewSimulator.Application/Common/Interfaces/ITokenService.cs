namespace AIInterviewSimulator.Application.Common.Interfaces;

public interface ITokenService
{
    Task<TokenResult> GenerateAccessTokenAsync(Guid userId);
}

public sealed record TokenResult(
    string AccessToken,
    DateTime ExpiresAtUtc
);