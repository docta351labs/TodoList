# AGENTS.md — AI Coding Guardrails & Conventions

> **Scope**: These rules apply to every AI agent (Antigravity, Copilot, Cursor, etc.) working on this repository.  
> Treat them as hard constraints, not suggestions. Violating them will trigger a review rejection.

---

## 1. Repository Layout

```
TodoList/
├── backend/
│   ├── src/
│   │   ├── TodoList.Api/            # ASP.NET Core Web API host
│   │   ├── TodoList.Application/    # CQRS handlers, DTOs, validators
│   │   ├── TodoList.Domain/         # Entities, Value Objects, Domain Events
│   │   └── TodoList.Infrastructure/ # EF Core, repositories, migrations
│   └── tests/
│       ├── TodoList.UnitTests/      # Domain + Application layer tests
│       └── TodoList.IntegrationTests/ # API + EF Core tests (Testcontainers)
├── frontend/
│   ├── src/
│   │   ├── api/        # Axios client + React Query hooks
│   │   ├── components/ # Reusable UI atoms/molecules
│   │   ├── features/   # Feature-sliced modules (todos/)
│   │   ├── pages/      # Route-level page components
│   │   ├── store/      # Zustand slices (UI-only state)
│   │   └── types/      # Shared TypeScript interfaces
│   └── tests/          # Vitest + React Testing Library
├── docs/
│   ├── adr/            # Architecture Decision Records (ADR-001, ADR-002 …)
│   └── design.md       # High-level architecture & domain design
├── .editorconfig
├── .gitignore
├── AGENTS.md           # this file
└── README.md
```

**Rules:**
- Never place business logic in `TodoList.Api` or `TodoList.Infrastructure`.
- Never place persistence details (DbContext, SQL) in `TodoList.Domain` or `TodoList.Application`.
- All new backend features must start with a domain entity or value object.

---

## 2. Language & Runtime Versions

| Layer      | Technology       | Version       |
|------------|------------------|---------------|
| Backend    | .NET             | 9.0 (LTS)     |
| ORM        | EF Core          | 9.x           |
| Database   | PostgreSQL       | 16            |
| Frontend   | Node.js          | 22 LTS        |
| Frontend   | React            | 19            |
| Frontend   | TypeScript       | 5.x (strict)  |
| Build tool | Vite             | 6.x           |

Do **not** downgrade any of these without creating an ADR.

---

## 3. Backend Conventions

### 3.1 Architecture: Vertical Slice / Clean Architecture hybrid

- **Domain layer** — pure C#, zero infrastructure dependencies, zero NuGet packages beyond `MediatR.Contracts`.
- **Application layer** — MediatR commands/queries, FluentValidation validators, DTO records.
- **Infrastructure layer** — EF Core `AppDbContext`, repository implementations, external service adapters.
- **API layer** — minimal API endpoints grouped by feature (`/todos`), middleware, DI wiring.

### 3.2 SOLID Enforcement

| Principle | Enforcement |
|-----------|-------------|
| **S**RP   | One handler per Command/Query. One repository per aggregate root. |
| **O**CP   | Extend via new handlers/decorators, never modify existing pipeline behaviors. |
| **L**SP   | Interfaces must be fully implementable; no `NotImplementedException` stubs. |
| **I**SP   | `IRepository<T>` must not force callers to depend on methods they don't use. Prefer specific interfaces. |
| **D**IP   | All dependencies injected via constructor. No `new ConcreteClass()` in application/domain code. |

### 3.3 Test-Driven Development (TDD)

- **Red -> Green -> Refactor** cycle is mandatory for all new features.
- Write the failing test **before** writing implementation code.
- Test file must be committed in the same commit as (or before) production code.
- Test naming: `MethodOrScenario_StateUnderTest_ExpectedBehavior`.

### 3.4 Testing Stack

| Test type        | Framework                          | Target layer             |
|------------------|------------------------------------|---------------------------|
| Unit             | xUnit + FluentAssertions           | Domain, Application      |
| Integration      | xUnit + Testcontainers + WebApplicationFactory | API + EF Core |
| Architecture     | NetArchTest                        | Layer isolation rules    |

```csharp
// Correct style
[Fact]
public void AddTodoItem_WithValidTitle_ShouldRaiseDomainEvent()
{
    // Arrange
    var list = TodoList.Create("Work");

    // Act
    list.AddItem("Buy milk");

    // Assert
    list.DomainEvents
        .Should().ContainSingle()
        .Which.Should().BeOfType<TodoItemAddedEvent>();
}
```

### 3.5 EF Core / Persistence

- All entity mappings via **Fluent API** (`IEntityTypeConfiguration<T>`). No data annotations on domain entities.
- Migrations live in `TodoList.Infrastructure/Migrations/`.
- Never call `SaveChangesAsync()` inside a repository — call it in the Unit of Work (via handler decorator).
- All queries must use `AsNoTracking()` for read-only operations.
- Use `snake_case` for all PostgreSQL table/column names (via `UseSnakeCaseNamingConvention()`).

### 3.6 API Design

- REST, versioned via URL prefix: `/api/v1/...`
- Return `ProblemDetails` (RFC 7807) for all errors via global exception middleware.
- Use `Results.CreatedAtRoute` for POST 201 responses.
- Validate all incoming DTOs with FluentValidation before reaching handlers.
- All endpoints documented with OpenAPI (Scalar UI at `/scalar`).

### 3.7 Nullable Reference Types

- `<Nullable>enable</Nullable>` is on for all projects.
- No `!` (null-forgiving) operators without a comment explaining why it's safe.

---

## 4. Frontend Conventions

### 4.1 Project Structure (Feature-Sliced Design)

```
features/todos/
  components/   # TodoList, TodoItem, AddTodoForm
  hooks/        # useTodos, useTodoMutations
  api.ts        # React Query query/mutation definitions
  types.ts      # Feature-local TS types
  index.ts      # Public barrel export
```

### 4.2 State Management

| State type       | Tool              |
|------------------|-------------------|
| Server state     | TanStack Query v5 |
| UI/local state   | React useState / useReducer |
| Global UI state  | Zustand (if needed) |

**Never** store server data in Zustand. React Query is the single source of truth for server state.

### 4.3 TypeScript Rules

- `"strict": true` in `tsconfig.json`.
- No `any` type. Use `unknown` and type guards instead.
- All API response shapes must be defined in `src/types/api.ts` and validated with Zod.

### 4.4 Component Rules

- Functional components only. No class components.
- Props interfaces named `[ComponentName]Props`.
- Co-locate CSS Modules with their component file.
- Keep components under 200 lines. Extract sub-components if exceeded.

### 4.5 Testing Stack

| Test type        | Framework                          |
|------------------|------------------------------------|  
| Unit / Component | Vitest + React Testing Library     |
| E2E              | Playwright (smoke tests only)      |

- Test files co-located with source: `ComponentName.test.tsx`.
- No snapshot tests. Assert on behavior, not markup.

### 4.6 Styling

- CSS Modules for component-scoped styles.
- Design tokens defined in `src/styles/tokens.css` (CSS custom properties).
- No inline styles except for truly dynamic values.

---

## 5. Git & PR Conventions

### Commit Message Format (Conventional Commits)

```
<type>(<scope>): <short description>

[optional body]
[optional footer]
```

**Types**: `feat`, `fix`, `test`, `refactor`, `docs`, `chore`, `ci`
**Scopes**: `api`, `domain`, `infra`, `frontend`, `db`, `ci`

**Examples:**
```
feat(domain): add TodoItem priority value object
test(api): add integration test for DELETE /todos/{id}
fix(infra): correct snake_case mapping for created_at column
```

### PR Requirements

- [ ] All existing tests pass (`dotnet test`, `npm run test`)
- [ ] New feature has corresponding unit test(s)
- [ ] No new compiler warnings
- [ ] OpenAPI spec regenerated if endpoints changed
- [ ] Migration added if schema changed
- [ ] ADR created if architectural decision was made

---

## 6. Security Guardrails

- **Never** commit secrets, connection strings, or API keys. Use `dotnet user-secrets` locally and environment variables in CI/CD.
- All endpoints require authentication (JWT Bearer) except `GET /health` and `GET /scalar`.
- Validate and sanitize all user input server-side regardless of client-side validation.
- Use parameterized queries only (EF Core handles this). No raw SQL string interpolation.
- Enable CORS only for the known frontend origin. No wildcard `*` in production.

---

## 7. Prohibited Patterns

The following are **banned** and will be flagged in review:

```csharp
// BAD: Static state / service locator
var service = ServiceLocator.Get<ITodoService>();

// BAD: God class / mixed responsibilities
public class TodoManager { /* handles DB, business logic, HTTP */ }

// BAD: Returning null from domain methods (use Result<T> or throw domain exception)
public TodoItem? Find(Guid id) => null;

// BAD: Catching and swallowing exceptions
catch (Exception) { /* ignored */ }

// BAD: Hardcoded connection strings
"Host=localhost;Database=todos;Username=postgres;Password=password"
```

```tsx
// BAD: Direct fetch() in components
useEffect(() => { fetch('/api/todos').then(/* ... */) }, []);

// BAD: Mutating state directly
todos.push(newTodo);

// BAD: any type
const data: any = await response.json();
```

---

## 8. Definition of Done

A feature is **Done** when:

1. Domain model updated and unit-tested (Red -> Green -> Refactor)
2. Application handler written with FluentValidation rules
3. API endpoint registered and documented in OpenAPI
4. Integration test covers happy path + at least one error path
5. Frontend hook + component implemented and component-tested
6. EF Core migration applied and tested
7. `README.md` updated if setup steps changed
8. No warnings in `dotnet build` or `npm run build`
