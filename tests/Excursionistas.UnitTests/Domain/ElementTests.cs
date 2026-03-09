using FluentAssertions;
using Excursionistas.Domain.Entities;

namespace Excursionistas.UnitTests.Domain;

public class ElementTests
{
    [Fact]
    public void Element_ShouldCreateWithProperties_WhenParametersAreValid()
    {
        // Arrange & Act
        var element = new Element
        {
            Name = "Manzana",
            Weight = 5.0m,
            Calories = 100m
        };

        // Assert
        element.Name.Should().Be("Manzana");
        element.Weight.Should().Be(5.0m);
        element.Calories.Should().Be(100m);
        element.IsActive.Should().BeTrue();
        element.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenAllPropertiesAreValid()
    {
        // Arrange
        var element = new Element
        {
            Name = "Manzana",
            Weight = 5.0m,
            Calories = 100m
        };

        // Act
        var isValid = element.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenNameIsEmpty()
    {
        // Arrange
        var element = new Element
        {
            Name = "",
            Weight = 5.0m,
            Calories = 100m
        };

        // Act
        var isValid = element.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenWeightIsZeroOrNegative()
    {
        // Arrange
        var element = new Element
        {
            Name = "Manzana",
            Weight = 0m,
            Calories = 100m
        };

        // Act
        var isValid = element.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenCaloriesIsZeroOrNegative()
    {
        // Arrange
        var element = new Element
        {
            Name = "Manzana",
            Weight = 5.0m,
            Calories = 0m
        };

        // Act
        var isValid = element.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void CalorieEfficiency_ShouldCalculateCorrectly()
    {
        // Arrange
        var element = new Element
        {
            Name = "Manzana",
            Weight = 5.0m,
            Calories = 100m
        };

        // Act
        var efficiency = element.CalorieEfficiency;

        // Assert
        efficiency.Should().Be(20m); // 100 / 5
    }

    [Fact]
    public void CalorieEfficiency_ShouldReturnZero_WhenWeightIsZero()
    {
        // Arrange
        var element = new Element
        {
            Name = "Manzana",
            Weight = 0m,
            Calories = 100m
        };

        // Act
        var efficiency = element.CalorieEfficiency;

        // Assert
        efficiency.Should().Be(0m);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var element = new Element
        {
            Name = "Manzana",
            Weight = 5.0m,
            Calories = 100m
        };

        // Act
        var stringRepresentation = element.ToString();

        // Assert
        stringRepresentation.Should().Contain("Manzana");
        stringRepresentation.Should().ContainAny("5.0", "5,0"); // Support both decimal formats
        stringRepresentation.Should().Contain("100");
    }
}
