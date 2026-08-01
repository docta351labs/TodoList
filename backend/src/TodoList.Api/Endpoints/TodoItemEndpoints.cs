using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Features.TodoItems.Commands.AddTodoItem;
using TodoList.Application.Features.TodoItems.Commands.DeleteTodoItem;
using TodoList.Application.Features.TodoItems.Commands.UpdateTodoItem;
using TodoList.Application.Features.TodoItems.Commands.UpdateTodoItemStatus;
using TodoList.Domain.Enums;

namespace TodoList.Api.Endpoints;

public static class TodoItemEndpoints
{
    public static RouteGroupBuilder MapTodoItemEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/{listId:guid}/items", async (Guid listId, [FromBody] AddTodoItemRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new AddTodoItemCommand(listId, request.Title, request.Description, request.Priority, request.DueDate);
            var result = await sender.Send(command, ct);
            return Results.Created($"/api/v1/todolists/{listId}/items/{result.Id}", result);
        })
        .WithName("AddTodoItem")
        .WithSummary("Add a new todo item to a list");

        group.MapPut("/{listId:guid}/items/{itemId:guid}", async (Guid listId, Guid itemId, [FromBody] UpdateTodoItemRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateTodoItemCommand(listId, itemId, request.Title, request.Description, request.Priority, request.DueDate);
            var result = await sender.Send(command, ct);
            return Results.Ok(result);
        })
        .WithName("UpdateTodoItem")
        .WithSummary("Update details of a todo item");

        group.MapPatch("/{listId:guid}/items/{itemId:guid}/status", async (Guid listId, Guid itemId, [FromBody] UpdateTodoItemStatusRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateTodoItemStatusCommand(listId, itemId, request.NewStatus);
            var result = await sender.Send(command, ct);
            return Results.Ok(result);
        })
        .WithName("UpdateTodoItemStatus")
        .WithSummary("Update status of a todo item");

        group.MapDelete("/{listId:guid}/items/{itemId:guid}", async (Guid listId, Guid itemId, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new DeleteTodoItemCommand(listId, itemId), ct);
            return Results.NoContent();
        })
        .WithName("DeleteTodoItem")
        .WithSummary("Delete a todo item from a list");

        return group;
    }
}

public record AddTodoItemRequest(string Title, string? Description, Priority Priority, DateOnly? DueDate);
public record UpdateTodoItemRequest(string Title, string? Description, Priority Priority, DateOnly? DueDate);
public record UpdateTodoItemStatusRequest(Status NewStatus);
