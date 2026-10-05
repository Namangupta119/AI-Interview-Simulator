using AIInterviewSimulator.Application.Common.Interfaces;
using AIInterviewSimulator.Infrastructure.AI.Configuration;
using AIInterviewSimulator.Infrastructure.AI.Services;
using AIInterviewSimulator.Infrastructure.Authentication;
using AIInterviewSimulator.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AIInterviewSimulator.Infrastructure.InterviewSessions;
using AIInterviewSimulator.Infrastructure.InterviewQuestions;

namespace AIInterviewSimulator.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database connection and context registration
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
        });

        // Register application abstraction to concrete context
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        // Configure Gemini settings from configuration
        services.Configure<GeminiSettings>(configuration.GetSection(GeminiSettings.SectionName));

        // Register AI Interview Service
        services.AddScoped<IAIInterviewService, GeminiInterviewService>();

        // Register Authentication services (Phase 4)
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IInterviewSessionService, InterviewSessionService>();
        services.AddScoped<IInterviewQuestionService, InterviewQuestionService>();

        // Configure JWT settings
        services.Configure<JwtSettings>(
        configuration.GetSection(JwtSettings.SectionName));

        var jwtSettings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException(
        "JwtSettings configuration is missing.");

        if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
        {
            throw new InvalidOperationException(
                "JWT SecretKey is not configured.");
        }

        if (jwtSettings.SecretKey.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT SecretKey must be at least 32 characters long.");
        }

        // Register JWT token service
        services.AddScoped<ITokenService, JwtTokenService>();

        // Configure JWT Bearer authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization();

        return services;
    }
}
