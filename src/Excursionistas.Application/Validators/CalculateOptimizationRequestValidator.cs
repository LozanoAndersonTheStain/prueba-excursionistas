using Excursionistas.Application.DTOs.Request;
using FluentValidation;

namespace Excursionistas.Application.Validators;

/// <summary>
/// Validador para CalculateOptimizationRequest utilizando FluentValidation.
/// </summary>
public class CalculateOptimizationRequestValidator : AbstractValidator<CalculateOptimizationRequest>
{
    public CalculateOptimizationRequestValidator()
    {
        RuleFor(x => x.MinimumCalories)
            .GreaterThan(0)
            .WithMessage("Minimum calories must be greater than 0")
            .LessThanOrEqualTo(100000)
            .WithMessage("Minimum calories cannot exceed 100000");

        RuleFor(x => x.MaximumWeight)
            .GreaterThan(0)
            .WithMessage("Maximum weight must be greater than 0")
            .LessThanOrEqualTo(10000)
            .WithMessage("Maximum weight cannot exceed 10000 units");

        RuleFor(x => x.ElementIds)
            .Must(ids => ids == null || ids.All(id => id > 0))
            .WithMessage("All element IDs must be greater than 0");
    }
}
