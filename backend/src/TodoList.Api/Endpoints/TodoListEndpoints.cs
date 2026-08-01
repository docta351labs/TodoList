using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Features.TodoLists.Commands.CreateTodoList;
using TodoList.Application.Features.TodoLists.Commands.DeleteTodoList;
using TodoList.Application.Features.TodoLists.Queries.GetTodoListById;
using TodoList.Application.Features.TodoLists.Queries.GetTodoLists;

namespace TodoList.Api.Endpoints;

public static class TodoListEndpoints
{
    public static RouteGroupBuilder MapTodoListEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var ownerId = GetOwnerId(user);
            var result = await sender.Send(new GetTodoListsQuery(ownerId), ct);
            return Results.Ok(result);
        })
        .WithName("GetTodoLists")
        .WithSummary("Get all todo lists for the authenticated user");

        group.MapPost("/", async ([FromBody] CreateTodoListRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var ownerId = GetOwnerId(user);
            var command = new CreateTodoListCommand(request.Title, ownerId);
            var result = await sender.Send(command, ct);
            return Results.CreatedAtRoute("GetTodoListById", new { id = result.Id }, result);
        })
        .WithName("CreateTodoList")
        .WithSummary("Create a new todo list");

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetTodoListByIdQuery(id), ct);
            return Results.Ok(result);
        })
        .WithName("GetTodoListById")
        .WithSummary("Get a todo list by ID with all items");

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new DeleteTodoListCommand(id), ct);
            return Results.NoContent();
        })
        .WithName("DeleteTodoList")
        .WithSummary("Delete a todo list by ID");

        return group;
    }

    private static Guid GetOwnerId(ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? user.FindFirst("sub")?.Value
                 ?? user.FindFirst("owner_id")?.Value;

        return Guid.TryParse(claim, out var ownerId)
            ? ownerId
            : Guid.Parse("00000000-0000-0000-0000-000000000001");
    }
}

public record CreateTodoListRequest(string Title);
