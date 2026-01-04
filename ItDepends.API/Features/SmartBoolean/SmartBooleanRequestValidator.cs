using FluentValidation;

namespace ItDepends.API.Features.SmartBoolean;

public class SmartBooleanRequestValidator : AbstractValidator<SmartBooleanRequest>
{
    public SmartBooleanRequestValidator()
    {
        RuleFor(x => x.Prompt)
            .NotEmpty()
            .WithMessage("Prompt is required.")
            .Length(1, 2000)
            .WithMessage("Prompt must be between 1 and 2000 characters.");
    }
}