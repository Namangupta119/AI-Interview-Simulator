using FluentValidation;
using AIInterviewSimulator.Application.Common.Models;
using AIInterviewSimulator.Domain.Enums;

namespace AIInterviewSimulator.Application.Interviews.Validators;

public sealed class CreateInterviewSessionRequestValidator
    : AbstractValidator<CreateInterviewSessionRequest>
{
    public CreateInterviewSessionRequestValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.TotalQuestions)
            .InclusiveBetween(1, 20);

        RuleFor(x => x.Topics)
            .NotNull();

        RuleFor(x => x.Topics)
            .Must(topics => topics.Distinct().Count() == topics.Count)
            .WithMessage("Duplicate interview topics are not allowed.");

        RuleForEach(x => x.Topics)
            .IsInEnum();

        RuleFor(x => x.CustomTopics)
            .Must(customTopics =>
                customTopics is null ||
                customTopics.Count <= 5)
            .WithMessage("A maximum of 5 custom topics is allowed.");

        RuleForEach(x => x.CustomTopics)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CustomTopics)
            .Must(customTopics =>
                customTopics is null ||
                customTopics
                    .Select(topic => topic.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Count() == customTopics.Count)
            .WithMessage("Duplicate custom topics are not allowed.");

        RuleFor(x => x)
            .Must(request =>
                (request.Topics?.Count ?? 0) +
                (request.CustomTopics?.Count ?? 0) > 0)
            .WithMessage("At least one interview topic is required.");

        RuleFor(x => x.ExperienceLevel)
            .IsInEnum();

        RuleFor(x => x.Difficulty)
            .IsInEnum();
    }
}