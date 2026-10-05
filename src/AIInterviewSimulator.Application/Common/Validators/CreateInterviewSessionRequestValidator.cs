using AIInterviewSimulator.Application.Common.Models;
using AIInterviewSimulator.Domain.Enums;
using FluentValidation;

namespace AIInterviewSimulator.Application.Common.Validators;

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
            .NotNull()
            .Must(topics => topics.Count > 0)
            .WithMessage("At least one interview topic is required.");

        RuleFor(x => x.Topics)
            .Must(topics => topics.Distinct().Count() == topics.Count)
            .WithMessage("Duplicate interview topics are not allowed.");

        RuleForEach(x => x.Topics)
            .IsInEnum();

        RuleFor(x => x.ExperienceLevel)
            .IsInEnum();

        RuleFor(x => x.Difficulty)
            .IsInEnum();
    }
}