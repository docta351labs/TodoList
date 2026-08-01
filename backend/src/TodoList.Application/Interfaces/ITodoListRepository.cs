using TodoList.Domain.Aggregates;

namespace TodoList.Application.Interfaces;

public interface ITodoListRepository
{
    Task<TodoListAggregate?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TodoListAggregate>> GetAllByOwnerIdAsync(Guid ownerId, CancellationToken ct = default);
    Task AddAsync(TodoListAggregate list, CancellationToken ct = default);
    Task UpdateAsync(TodoListAggregate list, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
