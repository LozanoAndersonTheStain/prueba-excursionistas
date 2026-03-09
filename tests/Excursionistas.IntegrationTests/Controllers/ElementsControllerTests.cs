using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Excursionistas.Application.DTOs.Request;
using Excursionistas.Application.DTOs.Response;
using Excursionistas.Infrastructure.Data;

namespace Excursionistas.IntegrationTests.Controllers;

public class ElementsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ElementsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoElements()
    {
        // Arrange
        await ClearDatabase();

        // Act
        var response = await _client.GetAsync("/api/elements");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var elements = await response.Content.ReadFromJsonAsync<IEnumerable<ElementResponse>>();
        elements.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_ShouldCreateElement_WhenRequestIsValid()
    {
        // Arrange
        await ClearDatabase();
        var request = new CreateElementRequest
        {
            Name = "Manzana",
            Weight = 5.0m,
            Calories = 100m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/elements", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var element = await response.Content.ReadFromJsonAsync<ElementResponse>();
        element.Should().NotBeNull();
        element!.Name.Should().Be(request.Name);
        element.Weight.Should().Be(request.Weight);
        element.Calories.Should().Be(request.Calories);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new CreateElementRequest
        {
            Name = "",
            Weight = -5.0m,
            Calories = -100m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/elements", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_ShouldReturnElement_WhenElementExists()
    {
        // Arrange
        await ClearDatabase();
        var createRequest = new CreateElementRequest
        {
            Name = "Manzana",
            Weight = 5.0m,
            Calories = 100m
        };

        var createResponse = await _client.PostAsJsonAsync("/api/elements", createRequest);
        var createdElement = await createResponse.Content.ReadFromJsonAsync<ElementResponse>();

        // Act
        var response = await _client.GetAsync($"/api/elements/{createdElement!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var element = await response.Content.ReadFromJsonAsync<ElementResponse>();
        element.Should().NotBeNull();
        element!.Id.Should().Be(createdElement.Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenElementDoesNotExist()
    {
        // Arrange
        var id = 999999;

        // Act
        var response = await _client.GetAsync($"/api/elements/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ShouldUpdateElement_WhenRequestIsValid()
    {
        // Arrange
        await ClearDatabase();
        var createRequest = new CreateElementRequest
        {
            Name = "Manzana",
            Weight = 5.0m,
            Calories = 100m
        };

        var createResponse = await _client.PostAsJsonAsync("/api/elements", createRequest);
        var createdElement = await createResponse.Content.ReadFromJsonAsync<ElementResponse>();

        var updateRequest = new UpdateElementRequest
        {
            Name = "Naranja",
            Weight = 7.0m,
            Calories = 150m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/elements/{createdElement!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedElement = await response.Content.ReadFromJsonAsync<ElementResponse>();
        updatedElement.Should().NotBeNull();
        updatedElement!.Name.Should().Be(updateRequest.Name);
        updatedElement.Weight.Should().Be(updateRequest.Weight);
        updatedElement.Calories.Should().Be(updateRequest.Calories);
    }

    [Fact]
    public async Task Delete_ShouldDeleteElement_WhenElementExists()
    {
        // Arrange
        await ClearDatabase();
        var createRequest = new CreateElementRequest
        {
            Name = "Manzana",
            Weight = 5.0m,
            Calories = 100m
        };

        var createResponse = await _client.PostAsJsonAsync("/api/elements", createRequest);
        var createdElement = await createResponse.Content.ReadFromJsonAsync<ElementResponse>();

        // Act
        var response = await _client.DeleteAsync($"/api/elements/{createdElement!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify element is deleted
        var getResponse = await _client.GetAsync($"/api/elements/{createdElement.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task ClearDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ExcursionistasDbContext>();
        context.Elements.RemoveRange(context.Elements);
        await context.SaveChangesAsync();
    }
}
