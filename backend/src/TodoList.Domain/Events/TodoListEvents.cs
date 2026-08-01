using TodoList.Domain.Enums;

namespace TodoList.Domain.Events;

public record TodoListCreatedEvent(Guid ListId, string Title, Guid OwnerId) : IDomainEvent;

public record TodoItemAddedEvent(Guid ListId, Guid ItemId) : IDomainEvent;

public record TodoItemStatusChangedEvent(Guid ListId, Guid ItemId, Status OldStatus, Status NewStatus) : IDomainEvent;

public record TodoItemDeletedEvent(Guid ListId, Guid ItemId) : IDomainEvent;

public record TodoListDeletedEvent(Guid ListId) : IDomainEvent;
