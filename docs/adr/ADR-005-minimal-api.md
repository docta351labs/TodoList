# ADR-005 - Use Minimal API over MVC Controllers

**Date**: 2026-08-01  
**Status**: Accepted

## Context

ASP.NET Core 9 supports two styles for defining HTTP endpoints: traditional MVC Controllers and Minimal API. We need to choose one for this project.

## Decision

Use **Minimal API** grouped by feature using `RouteGroupBuilder` and extension methods.

Each feature registers its own endpoints via a static extension method on `WebApplication`:

```csharp
// TodoList.Api/Endpoints/TodoListEndpoints.cs
public static class TodoListEndpoints
{
    public static RouteGroupBuilder MapTodoLists(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllTodoLists);
        group.MapPost("/", CreateTodoList);
        group.MapGet("/{id:guid}", GetTodoListById);
        group.MapDelete("/{id:guid}", DeleteTodoList);
        return group;
    }

    private static async Task<IResult> CreateTodoList(
        CreateTodoListRequest request,
        ISender sender,
        CancellationToken ct)
    {
        var command = new CreateTodoListCommand(request.Title);
        var result = await sender.Send(command, ct);
        return Results.CreatedAtRoute("GetTodoListById", new { id = result.Id }, result);
    }
    // ...
}

// Program.cs
var api = app.MapGroup("/api/v1").RequireAuthorization();
api.MapGroup("/todolists").MapTodoLists();
```

## Consequences

**Positive:**
- Less ceremony than MVC (no `[ApiController]`, `[Route]`, `[HttpGet]` attributes).
- Native OpenAPI support in .NET 9 works well with Minimal API.
- Endpoints are easier to unit-test in isolation.
- `ISender` injection keeps handler calls clean.

**Negative:**
- Large APIs can become unwieldy without disciplined grouping (mitigated by extension methods).
- Some developers prefer the convention-based discovery of MVC controllers.

## Alternatives Rejected

| Alternative | Reason rejected |
|-------------|----------------|
| MVC Controllers | More boilerplate; attribute-heavy; no clear advantage for this API size |
| Carter library | Additional dependency; Minimal API native grouping is sufficient |
