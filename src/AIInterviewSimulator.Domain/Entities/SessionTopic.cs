using AIInterviewSimulator.Domain.Enums;

namespace AIInterviewSimulator.Domain.Entities;

public class SessionTopic
{
    public int Id { get; set; }

    public Guid SessionId { get; set; }

    public InterviewSession? Session { get; set; }

    public InterviewTopic? Topic { get; set; }

    public string? CustomTopic { get; set; }
}