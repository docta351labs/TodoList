# Phase 3 — Application Layer (`TodoList.Application`)

> **Process**: Red → Green → Refactor. Unit tests use **NSubstitute** to mock repositories.  
> **Constraint**: No references to `Infrastructure`, `Api`, or EF Core.

---

## Interfaces & Contracts

- [ ] **3.1** Create `ITodoListRepository` interface (`Application/Interfaces/ITodoListRepository.cs`)
  - `Task<TodoList?> GetByIdAsync(Guid id, CancellationToken ct)`
  - `Task<IReadOnlyList<TodoList>> GetAllByOwnerIdAsync(Guid ownerId, CancellationToken ct)`
  - `Task AddAsync(TodoList list, CancellationToken ct)`
  - `Task UpdateAsync(TodoList list, CancellationToken ct)`
  - `Task DeleteAsync(Guid id, CancellationToken ct)`
- [ ] **3.2** Create `IUnitOfWork` interface (`Application/Interfaces/IUnitOfWork.cs`)
  - `Task<int> SaveChangesAsync(CancellationToken ct)`

---

## DTOs

- [ ] **3.3** Create `TodoItemDto` record (`Application/DTOs/TodoItemDto.cs`)
- [ ] **3.4** Create `TodoListDto` record (`Application/DTOs/TodoListDto.cs`)
- [ ] **3.5** Create `TodoListSummaryDto` record (for list-of-lists, without items)

---

## MediatR Pipeline Behaviors

- [ ] **3.6** Create `ValidationBehavior<TRequest, TResponse>` (`Application/Behaviors/ValidationBehavior.cs`)
  - Runs all `IValidator<TRequest>` for the request
  - Throws `ValidationException` (FluentValidation) if any rule fails
- [ ] **3.7** Create `UnitOfWorkBehavior<TRequest, TResponse>` (`Application/Behaviors/UnitOfWorkBehavior.cs`)
  - Calls `IUnitOfWork.SaveChangesAsync()` after the handler succeeds
  - Only runs for `ICommand` marker (not queries)

---

## Commands

- [ ] **3.8** `CreateTodoListCommand` + `CreateTodoListCommandHandler` + `CreateTodoListValidator`
  - Input: `Title (string)`, `OwnerId (Guid)` *(OwnerId injected from mock auth context in v1)*
  - Output: `TodoListDto`
  - Validator: Title not empty, max 100 chars
- [ ] **3.9** `AddTodoItemCommand` + `AddTodoItemCommandHandler` + `AddTodoItemValidator`
  - Input: `ListId`, `Title`, `Description?`, `Priority`, `DueDate?`
  - Output: `TodoItemDto`
  - Validator: Title not empty, max 200 chars; Description max 1000 chars
- [ ] **3.10** `UpdateTodoItemCommand` + `UpdateTodoItemCommandHandler` + `UpdateTodoItemValidator`
  - Input: `ListId`, `ItemId`, `Title`, `Description?`, `Priority`, `DueDate?`
  - Output: `TodoItemDto`
- [ ] **3.11** `UpdateTodoItemStatusCommand` + `UpdateTodoItemStatusCommandHandler` + `UpdateTodoItemStatusValidator`
  - Input: `ListId`, `ItemId`, `NewStatus (Status enum)`
  - Output: `TodoItemDto`
  - Handler delegates status transition to domain (`Start()` / `Complete()`)
- [ ] **3.12** `DeleteTodoItemCommand` + `DeleteTodoItemCommandHandler`
  - Input: `ListId`, `ItemId`
  - Output: `Unit` (MediatR)
- [ ] **3.13** `DeleteTodoListCommand` + `DeleteTodoListCommandHandler`
  - Input: `ListId`
  - Output: `Unit` (MediatR)

---

## Queries

- [ ] **3.14** `GetTodoListsQuery` + `GetTodoListsQueryHandler`
  - Input: `OwnerId (Guid)` *(from mock auth context)*
  - Output: `IReadOnlyList<TodoListSummaryDto>`
- [ ] **3.15** `GetTodoListByIdQuery` + `GetTodoListByIdQueryHandler`
  - Input: `ListId (Guid)`
  - Output: `TodoListDto` (includes items)
  - Throws `NotFoundException` if list not found

---

## Unit Tests (`TodoList.UnitTests/Application/`)

- [ ] **3.T1** `CreateTodoListCommandHandler_ValidCommand_ShouldReturnTodoListDto`
- [ ] **3.T2** `CreateTodoListCommandHandler_ValidCommand_ShouldCallRepositoryAddAsync`
- [ ] **3.T3** `AddTodoItemCommandHandler_ValidCommand_ShouldReturnTodoItemDto`
- [ ] **3.T4** `AddTodoItemCommandHandler_ListNotFound_ShouldThrowNotFoundException`
- [ ] **3.T5** `UpdateTodoItemStatusCommandHandler_ValidTransition_ShouldCallRepositoryUpdateAsync`
- [ ] **3.T6** `DeleteTodoListCommandHandler_ValidId_ShouldCallRepositoryDeleteAsync`
- [ ] **3.T7** `CreateTodoListValidator_EmptyTitle_ShouldHaveValidationError`
- [ ] **3.T8** `CreateTodoListValidator_TitleExceeds100Chars_ShouldHaveValidationError`
- [ ] **3.T9** `AddTodoItemValidator_EmptyTitle_ShouldHaveValidationError`
- [ ] **3.T10** `AddTodoItemValidator_DescriptionExceeds1000Chars_ShouldHaveValidationError`

---

## Notes

- `OwnerId` is a `Guid` that in v1 is a **fixed mock value** (e.g., `Guid.Parse("00000000-0000-0000-0000-000000000001")`).
  - In production this will come from JWT claims.
  - Inject it via `ICurrentUserService` interface so it's swappable.
- Never call `SaveChangesAsync()` inside a repository — the `UnitOfWorkBehavior` handles it.

---

## Acceptance Criteria

- `dotnet test --filter "FullyQualifiedName~TodoList.UnitTests.Application"` — all green
- Zero references to `Microsoft.EntityFrameworkCore` in `TodoList.Application`
