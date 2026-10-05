namespace AIInterviewSimulator.Application.Common.Interfaces;

/// <summary>
/// Abstraction over password hashing so the Application layer remains
/// decoupled from any specific hashing library (e.g. BCrypt).
/// The concrete implementation lives in Infrastructure.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain-text password and returns the hashed value.
    /// </summary>
    string Hash(string plainTextPassword);

    /// <summary>
    /// Verifies a plain-text password against a previously hashed value.
    /// </summary>
    bool Verify(string plainTextPassword, string hashedPassword);
}
