using Microsoft.EntityFrameworkCore;
using TodoList.Application.Interfaces;
using TodoList.Domain.Aggregates;
using TodoList.Infrastructure.Persistence;

namespace TodoList.Infrastructure.Repositories;

public class TodoListRepository : ITodoListRepository
{
    private readonly AppDbContext _dbContext;

    public TodoListRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TodoListAggregate?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.TodoLists
            .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == id, ct);
    }

    public async Task<IReadOnlyList<TodoListAggregate>> GetAllByOwnerIdAsync(Guid ownerId, CancellationToken ct = default)
    {
        return await _dbContext.TodoLists
            .AsNoTracking()
            .Include(l => l.Items)
            .Where(l => l.OwnerId == ownerId)
            .ToListAsync(ct);
    }

    public async Task AddAsync(TodoListAggregate list, CancellationToken ct = default)
    {
        await _dbContext.TodoLists.AddAsync(list, ct);
    }

    public Task UpdateAsync(TodoListAggregate list, CancellationToken ct = default)
    {
        _dbContext.TodoLists.Update(list);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var list = await _dbContext.TodoLists.FirstOrDefaultAsync(l => l.Id == id, ct);
        if (list != null)
        {
            _dbContext.TodoLists.Remove(list);
        }
    }
}
