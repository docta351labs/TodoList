using TodoList.Domain.Aggregates;

namespace TodoList.Application.DTOs;

public record TodoListSummaryDto(
    Guid Id,
    string Title,
    Guid OwnerId,
    DateTimeOffset CreatedAt,
    int ItemCount)
{
    public static TodoListSummaryDto FromEntity(TodoListAggregate list)
    {
        return new TodoListSummaryDto(
            list.Id,
            list.Title,
            list.OwnerId,
            list.CreatedAt,
            list.Items.Count);
    }
}
