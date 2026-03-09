using FluentAssertions;
using FluentValidation.TestHelper;
using Excursionistas.Application.DTOs.Request;
using Excursionistas.Application.Validators;

namespace Excursionistas.UnitTests.Application.Validators;

public class CreateElementRequestValidatorTests
{
    private readonly CreateElementRequestValidator _validator;

    public CreateElementRequestValidatorTests()
    {
        _validator = new CreateElementRequestValidator();
    }

    [Fact]
    public void Should_HaveError_When_NameIsEmpty()
    {
        // Arrange
        var request = new CreateElementRequest
        {
            Name = "",
            Weight = 5.0m,
            Calories = 100m
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_HaveError_When_WeightIsNegative()
    {
        // Arrange
        var request = new CreateElementRequest
        {
            Name = "Manzana",
            Weight = -5.0m,
            Calories = 100m
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Weight);
    }

    [Fact]
    public void Should_HaveError_When_CaloriesIsNegative()
    {
        // Arrange
        var request = new CreateElementRequest
        {
            Name = "Manzana",
            Weight = 5.0m,
            Calories = -100m
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Calories);
    }

    [Fact]
    public void Should_NotHaveError_When_RequestIsValid()
    {
        // Arrange
        var request = new CreateElementRequest
        {
            Name = "Manzana",
            Weight = 5.0m,
            Calories = 100m
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
