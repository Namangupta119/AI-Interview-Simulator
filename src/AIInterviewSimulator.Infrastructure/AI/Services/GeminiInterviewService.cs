using System.Text.Json;
using System.Text.Json.Nodes;
using AIInterviewSimulator.Application.Common.Interfaces;
using AIInterviewSimulator.Application.Common.Models;
using AIInterviewSimulator.Infrastructure.AI.Configuration;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AIInterviewSimulator.Infrastructure.AI.Services;

public class GeminiInterviewService : IAIInterviewService
{
    private readonly GeminiSettings _settings;
    private readonly ILogger<GeminiInterviewService> _logger;

    public GeminiInterviewService(
        IOptions<GeminiSettings> settings,
        ILogger<GeminiInterviewService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    private Client CreateClient()
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            throw new InvalidOperationException(
                "Gemini API key is not configured. Set 'GeminiSettings:ApiKey' via User Secrets or environment variable.");
        }

        if (string.IsNullOrWhiteSpace(_settings.Model))
        {
            throw new InvalidOperationException(
                "Gemini Model is not configured. Set 'GeminiSettings:Model' in application configuration.");
        }

        return new Client(apiKey: _settings.ApiKey);
    }

    public async Task<GeneratedQuestionResult> GenerateQuestionAsync(
        GenerateQuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();

        var systemPrompt = "You are an expert technical interviewer conducting a structured technical interview. " +
            "Generate one relevant, engaging, and role-appropriate interview question based on the candidate's level, difficulty, and topics. " +
            "Ensure the output strictly conforms to the requested JSON schema.";

        var topicsList = string.Join(", ", request.Topics);
        var userPrompt = $"Target Role: {request.TargetRole}\n" +
            $"Experience Level: {request.ExperienceLevel}\n" +
            $"Difficulty: {request.Difficulty}\n" +
            $"Topics: {topicsList}\n" +
            $"Question {request.QuestionNumber} of {request.TotalQuestions}.\n";

        if (request.PreviousQuestions?.Count > 0)
        {
            userPrompt += "Previous questions in this session:\n";
            foreach (var pq in request.PreviousQuestions)
            {
                userPrompt += $"- Question: {pq.QuestionText}\n";
            }
            userPrompt += "Ensure the new question does NOT repeat previous topics or questions.\n";
        }

        const string schemaJson = """
        {
            "type": "object",
            "properties": {
                "questionText": { "type": "string" },
                "expectedAnswerPoints": { "type": "string" },
                "topic": { "type": "string" }
            },
            "required": ["questionText", "expectedAnswerPoints", "topic"]
        }
        """;

        var config = new GenerateContentConfig
        {
            SystemInstruction = new Content
            {
                Parts = new List<Part> { new Part { Text = systemPrompt } }
            },
            ResponseMimeType = "application/json",
            ResponseJsonSchema = JsonNode.Parse(schemaJson),
            Temperature = _settings.Temperature
        };

        var response = await client.Models.GenerateContentAsync(
            model: _settings.Model,
            contents: userPrompt,
            config: config
        );

        var responseText = response.Text ?? response.Candidates?[0]?.Content?.Parts?[0]?.Text
            ?? throw new InvalidOperationException("Empty response received from Gemini API.");

        var result = JsonSerializer.Deserialize<GeneratedQuestionResult>(
            responseText,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        ) ?? throw new InvalidOperationException("Failed to deserialize generated question result.");

        if (!result.IsValid)
        {
            throw new InvalidOperationException("Generated question output did not meet validation constraints.");
        }

        return result;
    }

    public async Task<EvaluatedAnswerResult> EvaluateAnswerAsync(
        EvaluateAnswerRequest request,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();

        var systemPrompt = "You are an objective technical interviewer evaluating a candidate's answer. " +
            "Score the answer from 0.0 to 10.0 (where 10.0 is perfect, 7.0 is good pass, 5.0 is average, <5 is unsatisfactory). " +
            "Provide clear, constructive feedback, specific strengths, weaknesses, and improvement suggestions. " +
            "Ensure the output strictly conforms to the requested JSON schema.";

        var userPrompt = $"Target Role: {request.TargetRole}\n" +
            $"Experience Level: {request.ExperienceLevel}\n" +
            $"Question Asked: {request.QuestionText}\n" +
            $"Key Expected Points: {request.ExpectedAnswerPoints}\n" +
            $"Candidate's Answer: {request.CandidateAnswerText}\n";

        const string schemaJson = """
        {
            "type": "object",
            "properties": {
                "score": { "type": "number" },
                "feedback": { "type": "string" },
                "strengths": {
                    "type": "array",
                    "items": { "type": "string" }
                },
                "weaknesses": {
                    "type": "array",
                    "items": { "type": "string" }
                },
                "improvements": {
                    "type": "array",
                    "items": { "type": "string" }
                }
            },
            "required": ["score", "feedback", "strengths", "weaknesses", "improvements"]
        }
        """;

        var config = new GenerateContentConfig
        {
            SystemInstruction = new Content
            {
                Parts = new List<Part> { new Part { Text = systemPrompt } }
            },
            ResponseMimeType = "application/json",
            ResponseJsonSchema = JsonNode.Parse(schemaJson),
            Temperature = 0.2
        };

        var response = await client.Models.GenerateContentAsync(
            model: _settings.Model,
            contents: userPrompt,
            config: config
        );

        var responseText = response.Text ?? response.Candidates?[0]?.Content?.Parts?[0]?.Text
            ?? throw new InvalidOperationException("Empty response received from Gemini API.");

        var result = JsonSerializer.Deserialize<EvaluatedAnswerResult>(
            responseText,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        ) ?? throw new InvalidOperationException("Failed to deserialize evaluated answer result.");

        if (!result.IsValid)
        {
            throw new InvalidOperationException("Evaluated answer output did not meet validation constraints.");
        }

        return result;
    }

    public async Task<InterviewSummaryResult> GenerateInterviewSummaryAsync(
        GenerateSummaryRequest request,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();

        var systemPrompt = "You are a senior technical hiring manager summarizing an overall technical interview performance. " +
            "Calculate an overall score between 0.0 and 10.0, synthesize comprehensive final feedback, list key strengths, and areas for improvement. " +
            "Ensure the output strictly conforms to the requested JSON schema.";

        var userPrompt = $"Target Role: {request.TargetRole}\n" +
            $"Experience Level: {request.ExperienceLevel}\n" +
            "Questions and Candidate Evaluations:\n";

        for (var i = 0; i < request.Answers.Count; i++)
        {
            var item = request.Answers[i];
            userPrompt += $"Q{i + 1}: {item.QuestionText}\n" +
                $"Candidate Answer: {item.CandidateAnswerText}\n" +
                $"Score: {item.Score}/10 | Feedback: {item.Feedback}\n\n";
        }

        const string schemaJson = """
        {
            "type": "object",
            "properties": {
                "overallScore": { "type": "number" },
                "summaryFeedback": { "type": "string" },
                "keyStrengths": {
                    "type": "array",
                    "items": { "type": "string" }
                },
                "areasForImprovement": {
                    "type": "array",
                    "items": { "type": "string" }
                }
            },
            "required": ["overallScore", "summaryFeedback", "keyStrengths", "areasForImprovement"]
        }
        """;

        var config = new GenerateContentConfig
        {
            SystemInstruction = new Content
            {
                Parts = new List<Part> { new Part { Text = systemPrompt } }
            },
            ResponseMimeType = "application/json",
            ResponseJsonSchema = JsonNode.Parse(schemaJson),
            Temperature = 0.2
        };

        var response = await client.Models.GenerateContentAsync(
            model: _settings.Model,
            contents: userPrompt,
            config: config
        );

        var responseText = response.Text ?? response.Candidates?[0]?.Content?.Parts?[0]?.Text
            ?? throw new InvalidOperationException("Empty response received from Gemini API.");

        var result = JsonSerializer.Deserialize<InterviewSummaryResult>(
            responseText,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        ) ?? throw new InvalidOperationException("Failed to deserialize interview summary result.");

        if (!result.IsValid)
        {
            throw new InvalidOperationException("Interview summary output did not meet validation constraints.");
        }

        return result;
    }
}
