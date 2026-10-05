namespace AIInterviewSimulator.Infrastructure.AI.Configuration;

public class GeminiSettings
{
    public const string SectionName = "GeminiSettings";

    /// <summary>
    /// Gemini API key loaded from User Secrets or environment variable (not in source control).
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Configured Gemini model identifier (e.g., "gemini-flash-latest", "gemini-2.5-flash").
    /// Completely configuration-driven; not hardcoded in source.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    public double Temperature { get; set; } = 0.4;
}
