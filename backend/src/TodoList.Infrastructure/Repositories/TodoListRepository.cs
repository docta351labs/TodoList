using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TodoList.Application.Interfaces;
using TodoList.Domain.Aggregates;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.Persistence;

namespace TodoList.Infrastructure.Repositories;

public class TodoListRepository(AppDbContext dbContext) : ITodoListRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<TodoListAggregate?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.TodoLists
            .AsNoTracking()
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
        EntityEntry<TodoListAggregate> entry = _dbContext.Entry(list);
        if (entry.State == EntityState.Detached)
        {
            _dbContext.TodoLists.Update(list);
        }
        else
        {
            foreach (TodoItem item in list.Items)
            {
                EntityEntry<TodoItem> itemEntry = _dbContext.Entry(item);
                if (itemEntry.State == EntityState.Detached)
                {
                    _dbContext.TodoItems.Add(item);
                    itemEntry.Property("TodoListId").CurrentValue = list.Id;
                }
            }
        }
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        TodoListAggregate? list = await _dbContext.TodoLists.FirstOrDefaultAsync(l => l.Id == id, ct);
        if (list != null)
        {
            _dbContext.TodoLists.Remove(list);
        }
    }
}
