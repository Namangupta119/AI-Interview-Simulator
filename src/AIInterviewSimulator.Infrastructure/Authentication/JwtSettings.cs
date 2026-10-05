namespace AIInterviewSimulator.Infrastructure.Authentication;

public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public int AccessTokenExpiryMinutes { get; set; } = 60;
}