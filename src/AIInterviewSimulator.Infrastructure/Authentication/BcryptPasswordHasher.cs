using AIInterviewSimulator.Application.Common.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace AIInterviewSimulator.Infrastructure.Authentication;

/// <summary>
/// BCrypt.Net-Next implementation of <see cref="IPasswordHasher"/>.
/// Uses work factor 12 for an appropriate balance of security and performance.
/// This class is the only place in the solution that references the BCrypt library.
/// </summary>
public sealed class BcryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    /// <inheritdoc />
    public string Hash(string plainTextPassword)
    {
        if (string.IsNullOrWhiteSpace(plainTextPassword))
            throw new ArgumentException("Password must not be empty.", nameof(plainTextPassword));

        return BC.HashPassword(plainTextPassword, workFactor: WorkFactor);
    }

    /// <inheritdoc />
    public bool Verify(string plainTextPassword, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(plainTextPassword))
            return false;

        if (string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        return BC.Verify(plainTextPassword, hashedPassword);
    }
}
