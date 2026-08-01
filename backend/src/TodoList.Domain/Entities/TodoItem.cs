using TodoList.Domain.Enums;
using TodoList.Domain.Exceptions;

namespace TodoList.Domain.Entities;

public class TodoItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public Priority Priority { get; private set; }
    public Status Status { get; private set; }
    public DateOnly? DueDate { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    private TodoItem()
    {
        // For EF Core
        Title = null!;
    }

    private TodoItem(Guid id, string title, string? description, Priority priority, DateOnly? dueDate, DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Item title cannot be empty.");
        }

        Id = id;
        Title = title;
        Description = description;
        Priority = priority;
        Status = Status.Pending;
        DueDate = dueDate;
        CreatedAt = createdAt;
        CompletedAt = null;
    }

    public static TodoItem Create(string title, string? description, Priority priority, DateOnly? dueDate)
    {
        return new TodoItem(Guid.NewGuid(), title, description, priority, dueDate, DateTimeOffset.UtcNow);
    }

    public void Start()
    {
        if (Status != Status.Pending)
        {
            throw new InvalidStatusTransitionException(Status, Status.InProgress);
        }

        Status = Status.InProgress;
    }

    public void Complete()
    {
        if (Status != Status.InProgress)
        {
            throw new InvalidStatusTransitionException(Status, Status.Done);
        }

        Status = Status.Done;
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void Update(string title, string? description, Priority priority, DateOnly? dueDate)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Item title cannot be empty.");
        }

        Title = title;
        Description = description;
        Priority = priority;
        DueDate = dueDate;
    }
}
