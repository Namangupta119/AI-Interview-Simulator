using AIInterviewSimulator.Application.Common.Models;
using FluentValidation;

namespace AIInterviewSimulator.Application.Common.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(128);
    }
}