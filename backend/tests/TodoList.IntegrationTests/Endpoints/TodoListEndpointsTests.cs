using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs;
using TodoList.IntegrationTests.Fixtures;

namespace TodoList.IntegrationTests.Endpoints;

[Collection("IntegrationTests")]
public class TodoListEndpointsTests
{
    private readonly HttpClient _client;

    public TodoListEndpointsTests(TodoListApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostTodoList_WithValidTitle_Returns201AndBody()
    {
        // Arrange
        var request = new { Title = "Project Alpha" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/todolists", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<TodoListDto>();
        body.Should().NotBeNull();
        body!.Title.Should().Be("Project Alpha");
        body.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task PostTodoList_WithEmptyTitle_Returns400ProblemDetails()
    {
        // Arrange
        var request = new { Title = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/todolists", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(400);
        problem.Title.Should().Be("Validation Error");
    }

    [Fact]
    public async Task PostTodoList_WithTitleOver100Chars_Returns400ProblemDetails()
    {
        // Arrange
        var request = new { Title = new string('Z', 101) };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/todolists", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(400);
    }

    [Fact]
    public async Task GetTodoLists_ReturnsOnlyListsForCurrentUser()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync("/api/v1/todolists", new { Title = "User List" });
        createResponse.EnsureSuccessStatusCode();

        // Act
        var response = await _client.GetAsync("/api/v1/todolists");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lists = await response.Content.ReadFromJsonAsync<List<TodoListSummaryDto>>();
        lists.Should().NotBeNull();
        lists.Should().Contain(l => l.Title == "User List");
    }

    [Fact]
    public async Task GetTodoListById_ExistingId_Returns200WithItems()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync("/api/v1/todolists", new { Title = "Details List" });
        var created = await createResponse.Content.ReadFromJsonAsync<TodoListDto>();

        // Act
        var response = await _client.GetAsync($"/api/v1/todolists/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<TodoListDto>();
        list.Should().NotBeNull();
        list!.Id.Should().Be(created.Id);
        list.Title.Should().Be("Details List");
    }

    [Fact]
    public async Task GetTodoListById_UnknownId_Returns404ProblemDetails()
    {
        // Arrange
        var unknownId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/todolists/{unknownId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(404);
    }

    [Fact]
    public async Task DeleteTodoList_ExistingId_Returns204()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync("/api/v1/todolists", new { Title = "To be deleted" });
        var created = await createResponse.Content.ReadFromJsonAsync<TodoListDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/todolists/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verifyResponse = await _client.GetAsync($"/api/v1/todolists/{created.Id}");
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTodoList_UnknownId_Returns404ProblemDetails()
    {
        // Arrange
        var unknownId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/todolists/{unknownId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
