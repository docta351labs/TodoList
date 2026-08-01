using TodoList.Domain.Aggregates;

namespace TodoList.Application.DTOs;

public record TodoListDto(
    Guid Id,
    string Title,
    Guid OwnerId,
    DateTimeOffset CreatedAt,
    IReadOnlyList<TodoItemDto> Items)
{
    public static TodoListDto FromEntity(TodoListAggregate list)
    {
        return new TodoListDto(
            list.Id,
            list.Title,
            list.OwnerId,
            list.CreatedAt,
            list.Items.Select(TodoItemDto.FromEntity).ToList());
    }
}
