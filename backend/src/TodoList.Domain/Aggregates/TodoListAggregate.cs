using TodoList.Domain.Common;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Domain.Events;
using TodoList.Domain.Exceptions;

namespace TodoList.Domain.Aggregates;

public class TodoListAggregate : AggregateRoot
{
    private readonly List<TodoItem> _items = [];

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public Guid OwnerId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public IReadOnlyList<TodoItem> Items => _items.AsReadOnly();

    private TodoListAggregate()
    {
        // For EF Core
        Title = null!;
    }

    private TodoListAggregate(Guid id, string title, Guid ownerId, DateTimeOffset createdAt)
    {
        ValidateTitle(title);

        Id = id;
        Title = title;
        OwnerId = ownerId;
        CreatedAt = createdAt;
    }

    public static TodoListAggregate Create(string title, Guid ownerId)
    {
        var list = new TodoListAggregate(Guid.NewGuid(), title, ownerId, DateTimeOffset.UtcNow);
        list.RaiseDomainEvent(new TodoListCreatedEvent(list.Id, list.Title, list.OwnerId));
        return list;
    }

    public TodoItem AddItem(string title, string? description, Priority priority, DateOnly? dueDate)
    {
        if (_items.Count >= 500)
        {
            throw new ItemLimitExceededException(500);
        }

        var item = TodoItem.Create(title, description, priority, dueDate);
        _items.Add(item);
        RaiseDomainEvent(new TodoItemAddedEvent(Id, item.Id));
        return item;
    }

    public void DeleteItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            RaiseDomainEvent(new TodoItemDeletedEvent(Id, itemId));
        }
    }

    public void UpdateTitle(string newTitle)
    {
        ValidateTitle(newTitle);
        Title = newTitle;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Title cannot be empty.");
        }

        if (title.Length > 100)
        {
            throw new DomainException("Title cannot exceed 100 characters.");
        }
    }
}
