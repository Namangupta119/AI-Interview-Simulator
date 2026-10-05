namespace AIInterviewSimulator.Domain.Enums;

/// <summary>
/// Integer values are stored in the Roles table (seeded by InitialCreate migration).
/// Admin = 1, Candidate = 2 — must stay in sync with RoleConfiguration seed data.
/// </summary>
public enum UserRoleType
{
    Admin = 1,
    Candidate = 2
}

public enum SessionStatus
{
    InProgress = 0,
    Completed = 1,
    Abandoned = 2
}

public enum ExperienceLevel
{
    Junior = 0,
    MidLevel = 1,
    Senior = 2,
    Lead = 3
}

public enum InterviewDifficulty
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}

public enum InterviewTopic
{
    CSharp = 0,
    DotNetCore = 1,
    EntityFrameworkCore = 2,
    SqlAndDatabases = 3,
    SystemDesign = 4,
    DataStructuresAndAlgorithms = 5,
    WebSecurity = 6,
    Microservices = 7,
    ReactAndFrontend = 8
}
