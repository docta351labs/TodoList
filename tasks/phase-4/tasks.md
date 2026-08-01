# Phase 4 — Infrastructure Layer (`TodoList.Infrastructure`)

> **Constraint**: Only this layer may reference EF Core and Npgsql.  
> All entity mappings use **Fluent API only** — no data annotations on domain entities.

---

## Tasks

### AppDbContext

- [x] **4.1** Create `AppDbContext : DbContext` (`Infrastructure/Persistence/AppDbContext.cs`)
  - `DbSet<TodoList> TodoLists`
  - `DbSet<TodoItem> TodoItems`
  - Applies all `IEntityTypeConfiguration<T>` from the same assembly automatically (`modelBuilder.ApplyConfigurationsFromAssembly(...)`)
  - Override `SaveChangesAsync` to dispatch domain events from aggregate roots (via `IMediator`) then clear them
- [x] **4.2** Register `AppDbContext` with Npgsql and `UseSnakeCaseNamingConvention()` in DI

### Entity Configurations (Fluent API)

- [x] **4.3** Create `TodoListConfiguration : IEntityTypeConfiguration<TodoList>` (`Infrastructure/Persistence/Configurations/TodoListConfiguration.cs`)
  - Table: `todo_lists`
  - `Id`: UUID primary key
  - `Title`: `VARCHAR(100) NOT NULL`
  - `OwnerId`: UUID, not null
  - `CreatedAt`: `TIMESTAMPTZ NOT NULL`
  - Owns `Items` as a collection of dependent entities (or separate table — see note)
- [x] **4.4** Create `TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>` (`Infrastructure/Persistence/Configurations/TodoItemConfiguration.cs`)
  - Table: `todo_items`
  - `Id`: UUID primary key
  - `Title`: `VARCHAR(200) NOT NULL`
  - `Description`: `TEXT`, nullable
  - `Priority`: `SMALLINT NOT NULL` (stored as integer)
  - `Status`: `SMALLINT NOT NULL` (stored as integer)
  - `DueDate`: `DATE`, nullable
  - `CreatedAt`: `TIMESTAMPTZ NOT NULL`
  - `CompletedAt`: `TIMESTAMPTZ`, nullable
  - FK to `todo_lists(id)` with `ON DELETE CASCADE`
- [x] **4.5** Add indexes:
  - `idx_todo_items_list_id` on `todo_items(todo_list_id)`
  - `idx_todo_lists_owner_id` on `todo_lists(owner_id)`

### Repository

- [x] **4.6** Implement `TodoListRepository : ITodoListRepository` (`Infrastructure/Repositories/TodoListRepository.cs`)
  - All write methods operate on tracked entities
  - All read methods use `.AsNoTracking()` (per AGENTS.md §3.5)
  - `GetByIdAsync`: include items via `.Include(l => l.Items)`
  - `GetAllByOwnerIdAsync`: filter by `OwnerId`, no items needed (summary)
- [x] **4.7** Implement `IUnitOfWork` on `AppDbContext` (or a thin wrapper) — exposes `SaveChangesAsync`

### Mock Auth Context

- [x] **4.8** Create `ICurrentUserService` interface in `Application/Interfaces/`
- [x] **4.9** Create `MockCurrentUserService : ICurrentUserService` in `Infrastructure/Services/`
  - Returns a fixed `OwnerId = Guid.Parse("00000000-0000-0000-0000-000000000001")`
  - Register as `Scoped` in DI
  - *(Swap with real JWT claim reader when auth is implemented)*

### EF Core Migration

- [x] **4.10** Generate initial migration: `dotnet ef migrations add InitialCreate --project backend/src/TodoList.Infrastructure --startup-project backend/src/TodoList.Api`
- [x] **4.11** Verify migration SQL matches the schema in [`docs/design.md §5`](../../docs/design.md)

### DI Registration

- [x] **4.12** Create `InfrastructureServiceExtensions.AddInfrastructure(IServiceCollection, IConfiguration)` static method
  - Registers `AppDbContext`, `ITodoListRepository`, `ICurrentUserService`, `IUnitOfWork`

---

## Notes

- `SaveChangesAsync()` is **never called in repositories** — only via `UnitOfWorkBehavior` in the MediatR pipeline (AGENTS.md §3.5).
- Domain events are dispatched inside `AppDbContext.SaveChangesAsync()` override **before** saving, so handlers can react within the same transaction if needed.
- `snake_case` is enforced globally via `UseSnakeCaseNamingConvention()` — do not set column names manually unless overriding.

---

## Acceptance Criteria

- Migration generates valid SQL (inspect the `Migrations/` output)
- `dotnet build` — zero warnings
- Zero references to `Npgsql` or `Microsoft.EntityFrameworkCore` in `TodoList.Domain` or `TodoList.Application`
