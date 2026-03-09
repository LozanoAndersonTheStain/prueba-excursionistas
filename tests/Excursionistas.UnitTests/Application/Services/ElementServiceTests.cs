using FluentAssertions;
using Moq;
using AutoMapper;
using Excursionistas.Application.Services;
using Excursionistas.Application.DTOs.Request;
using Excursionistas.Application.DTOs.Response;
using Excursionistas.Domain.Entities;
using Excursionistas.Domain.Interfaces;

namespace Excursionistas.UnitTests.Application.Services;

public class ElementServiceTests
{
    private readonly Mock<IElementRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ElementService _service;

    public ElementServiceTests()
    {
        _mockRepository = new Mock<IElementRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new ElementService(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedElements()
    {
        // Arrange
        var elements = new List<Element>
        {
            new Element { Id = 1, Name = "Manzana", Weight = 5.0m, Calories = 100m },
            new Element { Id = 2, Name = "Naranja", Weight = 7.0m, Calories = 150m }
        };

        var elementResponses = new List<ElementResponse>
        {
            new ElementResponse { Id = 1, Name = "Manzana", Weight = 5.0m, Calories = 100m },
            new ElementResponse { Id = 2, Name = "Naranja", Weight = 7.0m, Calories = 150m }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(elements);
        _mockMapper.Setup(m => m.Map<IEnumerable<ElementResponse>>(elements))
            .Returns(elementResponses);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(elementResponses);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMappedElement_WhenElementExists()
    {
        // Arrange
        var id = 1;
        var element = new Element { Id = id, Name = "Manzana", Weight = 5.0m, Calories = 100m };
        var elementResponse = new ElementResponse { Id = id, Name = "Manzana", Weight = 5.0m, Calories = 100m };

        _mockRepository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(element);
        _mockMapper.Setup(m => m.Map<ElementResponse>(element))
            .Returns(elementResponse);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().BeEquivalentTo(elementResponse);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenElementDoesNotExist()
    {
        // Arrange
        var id = 1;

        _mockRepository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Element?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAndReturnMappedElement()
    {
        // Arrange
        var request = new CreateElementRequest
        {
            Name = "Manzana",
            Weight = 5.0m,
            Calories = 100m
        };

        var element = new Element
        {
            Name = request.Name,
            Weight = request.Weight,
            Calories = request.Calories
        };

        var createdElement = new Element
        {
            Id = 1,
            Name = request.Name,
            Weight = request.Weight,
            Calories = request.Calories
        };

        var elementResponse = new ElementResponse
        {
            Id = 1,
            Name = request.Name,
            Weight = request.Weight,
            Calories = request.Calories
        };

        _mockMapper.Setup(m => m.Map<Element>(request))
            .Returns(element);
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Element>()))
            .ReturnsAsync(createdElement);
        _mockMapper.Setup(m => m.Map<ElementResponse>(createdElement))
            .Returns(elementResponse);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().BeEquivalentTo(elementResponse);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Element>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenElementIsDeleted()
    {
        // Arrange
        var id = 1;

        _mockRepository.Setup(r => r.DeleteAsync(id))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
    }
}
