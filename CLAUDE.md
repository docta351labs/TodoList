# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Full-stack Todo List app: ASP.NET Core 9 (Clean Architecture + CQRS) backend, React 19 + TypeScript frontend, PostgreSQL 16. Read `AGENTS.md` before writing any code — it is the authoritative, enforced set of conventions (layering rules, SOLID checklist, prohibited patterns, PR checklist, commit format) for every AI agent working on this repo. `docs/design.md` has the full domain model, API spec, and DB schema; `docs/adr/` has the architecture decisions (why Clean Architecture, why Postgres, why TanStack Query, why Testcontainers, why Minimal API over MVC).

Auth is currently mocked: `MockAuthMiddleware` (backend) assigns every request a fixed identity (`00000000-0000-0000-0000-000000000001`) unless a real claim is present, and `MockCurrentUserService`/`ICurrentUserService` surfaces it to the Application layer. Real JWT validation is not wired up yet.

## Commands

### Backend (`backend/`)

```bash
dotnet build                                    # build whole solution
dotnet test tests/TodoList.UnitTests            # unit tests (Domain + Application, no I/O)
dotnet test tests/TodoList.IntegrationTests     # integration tests — requires Docker (Testcontainers spins up real Postgres)
dotnet test                                     # everything
dotnet test --filter "FullyQualifiedName~TodoItemTests.MethodName"   # single test
dotnet ef database update                       # apply migrations (run from backend/src/TodoList.Api)
dotnet ef migrations add <Name>                  # new migration (run from backend/src/TodoList.Api)
```

Run the API from `backend/src/TodoList.Api`: `dotnet run` — serves on `http://localhost:7001`, Scalar UI at `/scalar`, health check at `/health`. Requires `ConnectionStrings:DefaultConnection` and `Jwt:Secret` via `dotnet user-secrets` (see README for exact commands) and Postgres/Seq running (`docker-compose up db seq` or the two `docker run` commands in the README).

### Frontend (`frontend/`)

```bash
npm run dev            # Vite dev server, http://localhost:5173
npm run build           # tsc -b && vite build
npm run lint            # eslint .
npm run format           # prettier --write .
npm run test             # vitest run
npm run test -- <pattern>  # single test file/pattern, e.g. npm run test -- TodoItemCard
npm run test:ui          # vitest with UI
npm run test:e2e         # playwright (needs backend + frontend both running)
```

### Full stack via Docker

`docker-compose up` brings up Postgres, Seq, the API (`:7001`), and the frontend (`:5173`) together.

## Backend architecture

Clean Architecture with strict one-way dependency flow, enforced by `NetArchTest` rules in [ArchitectureTests.cs](backend/tests/TodoList.IntegrationTests/Architecture/ArchitectureTests.cs):

```
TodoList.Domain  <--  TodoList.Application  <--  TodoList.Infrastructure
                                 ^
                                 |
                            TodoList.Api
```

- **Domain** (`backend/src/TodoList.Domain`) — no dependency on any other project. `TodoListAggregate` (in `Aggregates/`) is the aggregate root; `TodoItem` is an entity owned by it (never persisted/loaded independently). All mutation goes through aggregate methods (`AddItem`, `DeleteItem`, `UpdateTitle`; `TodoItem.Update`/`Start`/`Complete`) which enforce invariants (title length, 500-item cap, valid status transitions) and raise domain events (`Events/TodoListEvents.cs`) via `AggregateRoot.RaiseDomainEvent`. Domain exceptions (`DomainException`, `ItemLimitExceededException`, `InvalidStatusTransitionException`) signal rule violations — never return null from a domain method.

- **Application** (`backend/src/TodoList.Application`) — CQRS via MediatR. Each use case is a single **vertical-slice file** under `Features/<Aggregate>/{Commands,Queries}/<UseCase>/<UseCase>.cs` containing the record (`ICommand<TResponse>`/`IQuery<TResponse>` from `Common/CQRS.cs`), its `AbstractValidator<T>` (FluentValidation), and its `IRequestHandler<,>` together — see [UpdateTodoItem.cs](backend/src/TodoList.Application/Features/TodoItems/Commands/UpdateTodoItem/UpdateTodoItem.cs) as the reference shape. `Behaviors/ValidationBehavior.cs` and `Behaviors/UnitOfWorkBehavior.cs` are MediatR pipeline behaviors — validation runs before the handler, and `SaveChangesAsync` is called centrally by the UoW behavior after a successful command, **not** inside repositories or handlers. Handlers depend only on `Interfaces/` abstractions (`ITodoListRepository`, `IUnitOfWork`, `ICurrentUserService`) — never a concrete Infrastructure type. `NotFoundException` (`Exceptions/`) is thrown for missing aggregates/items and translated to a 404 ProblemDetails by API middleware.

- **Infrastructure** (`backend/src/TodoList.Infrastructure`) — `Persistence/AppDbContext.cs` (EF Core 9, Npgsql, `UseSnakeCaseNamingConvention()`), `Persistence/Configurations/*Configuration.cs` (Fluent API mappings — no data annotations on domain entities), `Repositories/TodoListRepository.cs` (one repository for the whole aggregate — `TodoItem` has no repository of its own), `Services/MockCurrentUserService.cs`. `DependencyInjection.cs` wires all of this behind the Application-layer interfaces. Migrations live in `Migrations/`.

- **Api** (`backend/src/TodoList.Api`) — Minimal API, no controllers. `Endpoints/TodoListEndpoints.cs` / `TodoItemEndpoints.cs` are extension methods on `RouteGroupBuilder` that map HTTP verbs straight to `ISender.Send(command/query)` and translate the result to `Results.Ok/Created/NoContent`. Routes are grouped under `/api/v1/todolists` in `Program.cs`. `Middleware/ExceptionHandlingMiddleware.cs` converts exceptions to RFC 7807 `ProblemDetails`; `Middleware/MockAuthMiddleware.cs` stands in for real JWT auth. `Program.cs` also wires Serilog, OpenTelemetry (traces + metrics, console exporter), CORS (origins from `Cors:AllowedOrigins` config, defaults to the Vite dev origin), health checks (`AddNpgSql`), and Scalar/OpenAPI (dev-only, at `/openapi` + `/scalar`).

New backend features start in Domain (entity/aggregate method + test), then an Application feature slice, then Infrastructure/Api wiring — see the TDD cycle and Definition of Done in `AGENTS.md`.

## Frontend architecture

Feature-sliced under `frontend/src/`:

- `features/todos/` — the one real feature. `components/` (e.g. `TodoItemCard`, `AddTodoItemForm`, `TodoListCard`, `TodoItemStatusControl`) each pair a `.tsx` with a co-located CSS Module and, where behavior is tested, a `.test.tsx`. `hooks/` holds TanStack Query hooks (`useTodoList`, `useTodoLists`, `useAddTodoItem`, ...) — these are the only place components should reach the server; no direct `fetch`/`axios` in components. `api.ts` defines the query/mutation functions the hooks wrap. `index.ts` is the feature's public barrel export.
- `api/client.ts` — shared Axios instance; `api/queryClient.ts` — shared TanStack `QueryClient`; `api/todoApi.ts` — low-level HTTP calls.
- `pages/` — route-level components (`DashboardPage`, `ListDetailPage`) composed from `features/todos`.
- `components/` — cross-feature shared UI (`ErrorBoundary`, `LoadingSpinner`, `Toast`).
- `store/uiStore.ts` — Zustand, UI-only state. Server state always lives in TanStack Query, never in Zustand.
- `types/api.ts` — API response shapes, validated at the boundary with Zod (see `tests/apiSchemas.test.ts`).
- `tests/` — Vitest + RTL setup: `mocks/` and `handlers/` configure MSW to intercept API calls in tests; `testUtils.tsx` provides a custom render wrapped with the required providers.

TypeScript is strict, no `any` (use `unknown` + narrowing). Components stay under ~200 lines; split when they grow past that. Mutations use TanStack Query optimistic updates with rollback on error (see `docs/design.md` §6.3 for the intended pattern).

## Task planning docs

`tasks/phase-0` … `tasks/phase-12` contain phase-by-phase task breakdowns (`tasks.md`) used to plan the build. Check the relevant phase folder for prior intent if a task's scope or ordering is unclear.
