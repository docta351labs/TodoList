# Phase 2 — Domain Layer (`TodoList.Domain`)

> **Constraint**: No NuGet packages. Pure C# with zero infrastructure dependencies.  
> **Process**: Red → Green → Refactor. Write the failing unit test **before** writing production code.

---

## Production Code

### Base Types

- [ ] **2.1** Create `IDomainEvent` marker interface (`Domain/Events/IDomainEvent.cs`)
- [ ] **2.2** Create `AggregateRoot` base class (`Domain/Common/AggregateRoot.cs`)
  - Holds an internal `List<IDomainEvent>` exposed as `IReadOnlyList<IDomainEvent> DomainEvents`
  - Provides protected `RaiseDomainEvent(IDomainEvent)` method
  - Provides `ClearDomainEvents()` method (called after persistence)

### Enumerations

- [ ] **2.3** Create `Priority` enum (`Domain/Enums/Priority.cs`): `Low = 0`, `Medium = 1`, `High = 2`
- [ ] **2.4** Create `Status` enum (`Domain/Enums/Status.cs`): `Pending = 0`, `InProgress = 1`, `Done = 2`

### Domain Exceptions

- [ ] **2.5** Create `DomainException : Exception` (`Domain/Exceptions/DomainException.cs`)
- [ ] **2.6** Create `InvalidStatusTransitionException : DomainException`
- [ ] **2.7** Create `ItemLimitExceededException : DomainException`

### TodoItem Entity

- [ ] **2.8** Create `TodoItem` entity (`Domain/Entities/TodoItem.cs`)
  - Properties: `Id (Guid)`, `Title (string)`, `Description (string?)`, `Priority`, `Status`, `DueDate (DateOnly?)`, `CreatedAt (DateTimeOffset)`, `CompletedAt (DateTimeOffset?)`
  - Private constructor + static `Create(...)` factory method
  - `Start()`: `Pending → InProgress` only; throws `InvalidStatusTransitionException` otherwise
  - `Complete()`: `InProgress → Done` only; sets `CompletedAt = DateTimeOffset.UtcNow`; throws `InvalidStatusTransitionException` otherwise
  - `Update(title, description, priority, dueDate)` method

### TodoList Aggregate Root

- [ ] **2.9** Create `TodoList` aggregate root (`Domain/Aggregates/TodoList.cs`)
  - Properties: `Id (Guid)`, `Title (string)`, `OwnerId (Guid)`, `CreatedAt (DateTimeOffset)`, `Items (IReadOnlyList<TodoItem>)`
  - Private constructor + `Create(string title, Guid ownerId)` factory
  - Title validation: 1–100 chars; throws `DomainException` if violated
  - `AddItem(string title, string? description, Priority priority, DateOnly? dueDate)`: enforces 500-item limit; raises `TodoItemAddedEvent`
  - `DeleteItem(Guid itemId)`: removes item; raises `TodoItemDeletedEvent`

### Domain Events

- [ ] **2.10** Create `TodoListCreatedEvent(Guid ListId, string Title, Guid OwnerId)` record
- [ ] **2.11** Create `TodoItemAddedEvent(Guid ListId, Guid ItemId)` record
- [ ] **2.12** Create `TodoItemStatusChangedEvent(Guid ListId, Guid ItemId, Status OldStatus, Status NewStatus)` record
- [ ] **2.13** Create `TodoItemDeletedEvent(Guid ListId, Guid ItemId)` record
- [ ] **2.14** Create `TodoListDeletedEvent(Guid ListId)` record

---

## Unit Tests (`TodoList.UnitTests/Domain/`)

> Naming: `MethodOrScenario_StateUnderTest_ExpectedBehavior`

- [ ] **2.T1** `Create_WithValidTitle_ShouldRaiseTodoListCreatedEvent`
- [ ] **2.T2** `Create_WithEmptyTitle_ShouldThrowDomainException`
- [ ] **2.T3** `Create_WithTitleExceeding100Chars_ShouldThrowDomainException`
- [ ] **2.T4** `AddItem_WithValidTitle_ShouldRaiseTodoItemAddedEvent`
- [ ] **2.T5** `AddItem_WhenListHas500Items_ShouldThrowItemLimitExceededException`
- [ ] **2.T6** `Start_WhenStatusIsPending_ShouldTransitionToInProgress`
- [ ] **2.T7** `Start_WhenStatusIsNotPending_ShouldThrowInvalidStatusTransitionException`
- [ ] **2.T8** `Complete_WhenStatusIsInProgress_ShouldSetCompletedAt`
- [ ] **2.T9** `Complete_WhenStatusIsNotInProgress_ShouldThrowInvalidStatusTransitionException`
- [ ] **2.T10** `DeleteItem_WithExistingItem_ShouldRaiseTodoItemDeletedEvent`

---

## Acceptance Criteria

- `dotnet test --filter "FullyQualifiedName~TodoList.UnitTests.Domain"` — all green
- Zero references to `Microsoft.EntityFrameworkCore` or any infrastructure namespace in `TodoList.Domain`
