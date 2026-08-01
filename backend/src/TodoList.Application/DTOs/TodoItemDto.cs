using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs;

public record TodoItemDto(
    Guid Id,
    string Title,
    string? Description,
    Priority Priority,
    Status Status,
    DateOnly? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt)
{
    public static TodoItemDto FromEntity(TodoItem item)
    {
        return new TodoItemDto(
            item.Id,
            item.Title,
            item.Description,
            item.Priority,
            item.Status,
            item.DueDate,
            item.CreatedAt,
            item.CompletedAt);
    }
}
