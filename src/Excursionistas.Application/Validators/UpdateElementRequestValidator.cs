using Excursionistas.Application.DTOs.Request;
using FluentValidation;

namespace Excursionistas.Application.Validators;

/// <summary>
/// Validador para UpdateElementRequest utilizando FluentValidation.
/// </summary>
public class UpdateElementRequestValidator : AbstractValidator<UpdateElementRequest>
{
    public UpdateElementRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Element name is required")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than 0")
            .LessThanOrEqualTo(1000)
            .WithMessage("Weight cannot exceed 1000 units");

        RuleFor(x => x.Calories)
            .GreaterThan(0)
            .WithMessage("Calories must be greater than 0")
            .LessThanOrEqualTo(10000)
            .WithMessage("Calories cannot exceed 10000");
    }
}
