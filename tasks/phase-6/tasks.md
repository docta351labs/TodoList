# Phase 6 — Integration & Architecture Tests (`TodoList.IntegrationTests`)

> Integration tests use **Testcontainers** for real PostgreSQL (ADR-004).  
> Architecture tests use **NetArchTest** to enforce layer isolation (ADR-001).  
> Mock auth is active — no JWT tokens required for requests.

---

## Tasks

### Test Infrastructure

- [x] **6.1** Create `PostgreSqlContainerFixture : IAsyncLifetime` (`Fixtures/PostgreSqlContainerFixture.cs`)
  - Starts a `postgres:16-alpine` container
  - Exposes `ConnectionString`
  - Applies EF Core migrations on `InitializeAsync`
  - Stops container on `DisposeAsync`
- [x] **6.2** Create `TodoListApiFactory : WebApplicationFactory<Program>` (`Fixtures/TodoListApiFactory.cs`)
  - Overrides `ConnectionStrings:DefaultConnection` with container connection string
  - Removes the real `AppDbContext` registration and replaces it with test connection
  - Mock auth middleware remains active

### Integration Tests — TodoLists Endpoints

- [x] **6.3** `PostTodoList_WithValidTitle_Returns201AndBody`
- [x] **6.4** `PostTodoList_WithEmptyTitle_Returns400ProblemDetails`
- [x] **6.5** `PostTodoList_WithTitleOver100Chars_Returns400ProblemDetails`
- [x] **6.6** `GetTodoLists_ReturnsOnlyListsForCurrentUser`
- [x] **6.7** `GetTodoListById_ExistingId_Returns200WithItems`
- [x] **6.8** `GetTodoListById_UnknownId_Returns404ProblemDetails`
- [x] **6.9** `DeleteTodoList_ExistingId_Returns204`
- [x] **6.10** `DeleteTodoList_UnknownId_Returns404ProblemDetails`

### Integration Tests — TodoItems Endpoints

- [x] **6.11** `PostTodoItem_WithValidData_Returns201AndBody`
- [x] **6.12** `PostTodoItem_WithEmptyTitle_Returns400ProblemDetails`
- [x] **6.13** `PatchTodoItemStatus_ValidTransitionPendingToInProgress_Returns200`
- [x] **6.14** `PatchTodoItemStatus_InvalidTransition_Returns422ProblemDetails`
- [x] **6.15** `PutTodoItem_UpdatesFields_Returns200`
- [x] **6.16** `DeleteTodoItem_ExistingItem_Returns204`

### Architecture Tests

- [x] **6.17** `Domain_ShouldNotReference_Infrastructure`
- [x] **6.18** `Domain_ShouldNotReference_Application`
- [x] **6.19** `Application_ShouldNotReference_Infrastructure`
- [x] **6.20** `Application_ShouldNotReference_Api`
- [x] **6.21** `AllCommandHandlers_ShouldImplement_IRequestHandlerInterface`
- [x] **6.22** `AllQueryHandlers_ShouldImplement_IRequestHandlerInterface`

---

## Notes

- Each test class inherits from a shared base that manages `TodoListApiFactory` lifetime.
- Use `IClassFixture<TodoListApiFactory>` for sharing a single factory instance per class.
- Each test method should clean up data it creates (or use isolated data per test to avoid coupling).
- ProblemDetails assertions: check `status`, `title`, and `errors` fields.

---

## Acceptance Criteria

- `dotnet test tests/TodoList.IntegrationTests` — all green
- `dotnet test tests/TodoList.UnitTests` — all still green
- NetArchTest assertions enforce layer boundaries
