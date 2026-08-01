using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs;
using TodoList.Domain.Enums;
using TodoList.IntegrationTests.Fixtures;

namespace TodoList.IntegrationTests.Endpoints;

public class TodoItemEndpointsTests : IClassFixture<TodoListApiFactory>
{
    private readonly HttpClient _client;

    public TodoItemEndpointsTests(TodoListApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostTodoItem_WithValidData_Returns201AndBody()
    {
        // Arrange
        var list = await CreateTodoListAsync("Item Test List");
        var itemRequest = new
        {
            Title = "Buy Milk",
            Description = "Whole milk",
            Priority = Priority.High,
            DueDate = (DateOnly?)DateOnly.FromDateTime(DateTime.Today.AddDays(2))
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/todolists/{list.Id}/items", itemRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var item = await response.Content.ReadFromJsonAsync<TodoItemDto>();
        item.Should().NotBeNull();
        item!.Title.Should().Be("Buy Milk");
        item.Status.Should().Be(Status.Pending);
        item.Priority.Should().Be(Priority.High);
    }

    [Fact]
    public async Task PostTodoItem_WithEmptyTitle_Returns400ProblemDetails()
    {
        // Arrange
        var list = await CreateTodoListAsync("Validation Test List");
        var itemRequest = new
        {
            Title = "",
            Description = "Description",
            Priority = Priority.Low,
            DueDate = (DateOnly?)null
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/todolists/{list.Id}/items", itemRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(400);
    }

    [Fact]
    public async Task PatchTodoItemStatus_ValidTransitionPendingToInProgress_Returns200()
    {
        // Arrange
        var list = await CreateTodoListAsync("Status Test List");
        var item = await AddTodoItemAsync(list.Id, "Task 1");

        var patchRequest = new { NewStatus = Status.InProgress };

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/v1/todolists/{list.Id}/items/{item.Id}/status", patchRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<TodoItemDto>();
        updated.Should().NotBeNull();
        updated!.Status.Should().Be(Status.InProgress);
    }

    [Fact]
    public async Task PatchTodoItemStatus_InvalidTransition_Returns422ProblemDetails()
    {
        // Arrange
        var list = await CreateTodoListAsync("Invalid Status List");
        var item = await AddTodoItemAsync(list.Id, "Pending Task"); // Pending status

        // Trying to jump directly from Pending to Done without InProgress
        var patchRequest = new { NewStatus = Status.Done };

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/v1/todolists/{list.Id}/items/{item.Id}/status", patchRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(422);
        problem.Title.Should().Be("Domain Rule Violation");
    }

    [Fact]
    public async Task PutTodoItem_UpdatesFields_Returns200()
    {
        // Arrange
        var list = await CreateTodoListAsync("Put Test List");
        var item = await AddTodoItemAsync(list.Id, "Old Title");

        var updateRequest = new
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Priority = Priority.Low,
            DueDate = (DateOnly?)DateOnly.FromDateTime(DateTime.Today.AddDays(5))
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/todolists/{list.Id}/items/{item.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<TodoItemDto>();
        updated.Should().NotBeNull();
        updated!.Title.Should().Be("Updated Title");
        updated.Description.Should().Be("Updated Description");
        updated.Priority.Should().Be(Priority.Low);
    }

    [Fact]
    public async Task DeleteTodoItem_ExistingItem_Returns204()
    {
        // Arrange
        var list = await CreateTodoListAsync("Delete Item List");
        var item = await AddTodoItemAsync(list.Id, "Item to Delete");

        // Act
        var response = await _client.DeleteAsync($"/api/v1/todolists/{list.Id}/items/{item.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<TodoListDto> CreateTodoListAsync(string title)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/todolists", new { Title = title });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TodoListDto>())!;
    }

    private async Task<TodoItemDto> AddTodoItemAsync(Guid listId, string title)
    {
        var response = await _client.PostAsJsonAsync($"/api/v1/todolists/{listId}/items", new
        {
            Title = title,
            Description = (string?)null,
            Priority = Priority.Medium,
            DueDate = (DateOnly?)null
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TodoItemDto>())!;
    }
}
