# Design Document — TodoList Application

> **Version**: 1.0.0  
> **Last updated**: 2026-08-01  
> **Authors**: Project team

---

## 1. Overview

TodoList is a full-stack web application that allows authenticated users to manage personal task lists. The backend is an ASP.NET Core 9 Web API following Clean Architecture principles with CQRS. The frontend is a React 19 SPA that consumes the API via TanStack Query.

### Goals

- Demonstrate production-grade .NET + React architecture.
- Enforce SOLID principles and TDD at every layer.
- Provide a reference implementation for onboarding new engineers.

### Non-Goals (v1)

- Real-time collaboration (WebSockets / SignalR).
- Mobile native apps.
- File attachments on tasks.

---

## 2. Domain Model

```
TodoList (Aggregate Root)
  ├── Id                 : Guid
  ├── Title              : string (1–100 chars)
  ├── OwnerId            : Guid (User reference)
  ├── CreatedAt          : DateTimeOffset
  ├── Items              : IReadOnlyList<TodoItem>
  └── DomainEvents       : IReadOnlyList<IDomainEvent>

TodoItem (Entity, owned by TodoList)
  ├── Id                 : Guid
  ├── Title              : string (1–200 chars)
  ├── Description        : string? (0–1000 chars)
  ├── Priority           : Priority (enum: Low | Medium | High)
  ├── Status             : Status (enum: Pending | InProgress | Done)
  ├── DueDate            : DateOnly?
  ├── CreatedAt          : DateTimeOffset
  └── CompletedAt        : DateTimeOffset?
```

### Domain Rules

| Rule | Enforcement location |
|------|----------------------|
| A list must have a non-empty title | `TodoList` constructor / `TodoList.Create()` factory |
| A list cannot contain more than 500 items | `TodoList.AddItem()` |
| Only Pending items can be moved to InProgress | `TodoItem.Start()` |
| Only InProgress items can be completed | `TodoItem.Complete()` |
| `CompletedAt` is set only when status becomes Done | `TodoItem.Complete()` |

### Domain Events

```
TodoListCreatedEvent
TodoItemAddedEvent
TodoItemStatusChangedEvent
TodoItemDeletedEvent
TodoListDeletedEvent
```

---

## 3. Architecture

### 3.1 High-Level Diagram

```
Browser (React SPA)
       |
       | HTTPS / JSON
       v
[ASP.NET Core 9 API]  --- JWT validation ---> Identity Provider (future)
       |
       | MediatR pipeline
       v
[Application Layer]  (Commands / Queries / Validators)
       |
       | Repository interfaces (DIP)
       v
[Infrastructure Layer]  (EF Core 9, AppDbContext)
       |
       | Npgsql
       v
[PostgreSQL 16]
```

### 3.2 Request Pipeline (Backend)

```
HTTP Request
  --> Auth Middleware (JWT)
  --> Validation Middleware (FluentValidation via MediatR pipeline behavior)
  --> MediatR Handler
  --> Repository / AppDbContext
  --> SaveChanges (Unit of Work behavior)
HTTP Response (200/201/204/4xx/5xx + ProblemDetails)
```

### 3.3 CQRS Split

| Operation | Type | Handler |
|-----------|------|---------|
| Create a list | Command | `CreateTodoListCommandHandler` |
| Add an item | Command | `AddTodoItemCommandHandler` |
| Update item status | Command | `UpdateTodoItemStatusCommandHandler` |
| Update item fields | Command | `UpdateTodoItemCommandHandler` |
| Delete an item | Command | `DeleteTodoItemCommandHandler` |
| Delete a list | Command | `DeleteTodoListCommandHandler` |
| Get all lists for user | Query | `GetTodoListsQueryHandler` |
| Get list with items | Query | `GetTodoListByIdQueryHandler` |

---

## 4. API Specification

### Base URL

```
/api/v1
```

### Endpoints

#### TodoLists

| Method | Path | Description | Success |
|--------|------|-------------|---------|
| GET | `/todolists` | Get all lists for current user | 200 |
| POST | `/todolists` | Create a new list | 201 |
| GET | `/todolists/{id}` | Get list with all items | 200 |
| DELETE | `/todolists/{id}` | Delete a list (cascades items) | 204 |

#### TodoItems

| Method | Path | Description | Success |
|--------|------|-------------|---------|
| POST | `/todolists/{listId}/items` | Add an item to a list | 201 |
| PUT | `/todolists/{listId}/items/{itemId}` | Update item fields | 200 |
| PATCH | `/todolists/{listId}/items/{itemId}/status` | Transition item status | 200 |
| DELETE | `/todolists/{listId}/items/{itemId}` | Delete an item | 204 |

#### System

| Method | Path | Description |
|--------|------|-------------|
| GET | `/health` | Health check (no auth required) |
| GET | `/scalar` | OpenAPI docs (no auth required) |

### Error Responses

All errors use [RFC 7807 ProblemDetails](https://www.rfc-editor.org/rfc/rfc7807):

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-abc123-def456-00",
  "errors": {
    "Title": ["Title must not be empty."]
  }
}
```

### Sample Request/Response

**POST /api/v1/todolists**

Request:
```json
{ "title": "Shopping" }
```

Response 201:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Shopping",
  "ownerId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "createdAt": "2026-08-01T19:00:00Z",
  "items": []
}
```

---

## 5. Database Schema

```sql
-- snake_case enforced via EF Core UseSnakeCaseNamingConvention()

CREATE TABLE todo_lists (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title       VARCHAR(100) NOT NULL,
    owner_id    UUID NOT NULL,
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE todo_items (
    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    todo_list_id  UUID NOT NULL REFERENCES todo_lists(id) ON DELETE CASCADE,
    title         VARCHAR(200) NOT NULL,
    description   TEXT,
    priority      SMALLINT NOT NULL DEFAULT 1,  -- 0=Low,1=Medium,2=High
    status        SMALLINT NOT NULL DEFAULT 0,  -- 0=Pending,1=InProgress,2=Done
    due_date      DATE,
    created_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    completed_at  TIMESTAMPTZ
);

CREATE INDEX idx_todo_items_list_id ON todo_items(todo_list_id);
CREATE INDEX idx_todo_lists_owner_id ON todo_lists(owner_id);
```

---

## 6. Frontend Architecture

### 6.1 Page Structure

```
/ (root)
  /login        -> LoginPage
  /             -> DashboardPage (list of TodoLists)
  /lists/:id    -> ListDetailPage (items within a list)
```

### 6.2 Data Flow

```
ListDetailPage
  +-- useTodoList(listId)        <- TanStack Query (GET /todolists/:id)
        +-- TodoListView
              +-- AddTodoItemForm
              |     +-- useAddTodoItem()   <- useMutation (POST /items)
              +-- TodoItemList
                    +-- TodoItemCard
                          +-- useUpdateItemStatus()  <- useMutation (PATCH)
                          +-- useDeleteItem()        <- useMutation (DELETE)
```

### 6.3 Optimistic Updates

All mutations use TanStack Query optimistic updates:
- Immediately reflect the change in the UI.
- Roll back on error and show a toast notification.

### 6.4 Error Handling

- Network errors -> global error boundary with retry button.
- 4xx validation errors -> inline field error messages.
- 5xx server errors -> toast notification with correlation `traceId`.

---

## 7. Non-Functional Requirements

| Category        | Requirement |
|-----------------|-------------|
| Performance     | API p95 < 200ms for list queries |
| Availability    | 99.5% uptime target |
| Security        | JWT RS256, HTTPS-only, CORS restricted |
| Observability   | Structured logging (Serilog), OpenTelemetry traces |
| Scalability     | Stateless API; horizontal scaling ready |
| Accessibility   | WCAG 2.1 AA for all interactive elements |

---

## 8. Testing Strategy

### Backend

```
Unit Tests (fast, no I/O)
  Domain entities -> all business rules
  Application handlers -> mocked repositories (NSubstitute)
  Validators -> FluentValidation test extensions

Integration Tests (Testcontainers -- real PostgreSQL)
  Full HTTP round-trip via WebApplicationFactory
  EF Core migrations applied fresh per test class
  At minimum: happy path + validation error + not-found error

Architecture Tests (NetArchTest)
  Domain has no dependency on Infrastructure
  Application has no dependency on Api
  All handlers implement IRequestHandler<,>
```

### Frontend

```
Component Tests (Vitest + RTL)
  AddTodoItemForm -> submits correctly, shows validation errors
  TodoItemCard -> shows item data, triggers correct callbacks
  useTodos hook -> returns correct state from mock server (MSW)

E2E (Playwright -- smoke only)
  Login -> create list -> add item -> complete item -> delete list
```

---

## 9. Project Setup Checklist

### Backend

- [x] `dotnet new sln -n TodoList`
- [x] Create projects: `Api`, `Application`, `Domain`, `Infrastructure`, `UnitTests`, `IntegrationTests`
- [x] Add project references (Domain <- Application <- Infrastructure <- Api)
- [x] Install NuGet packages (see `docs/dependencies.md`)
- [x] Configure `appsettings.json` and `dotnet user-secrets`
- [x] Add initial EF Core migration
- [x] Configure Serilog + OpenTelemetry

### Frontend

- [x] `npm create vite@latest frontend -- --template react-ts`
- [x] Install dependencies (TanStack Query, Zod, Zustand, React Router, Axios)
- [x] Configure `tsconfig.json` (strict mode)
- [x] Set up Vitest + RTL + MSW
- [x] Configure Playwright
- [x] Add ESLint + Prettier

---

## 10. Architecture Decision Records

| ADR | Decision |
|-----|----------|
| [ADR-001](adr/ADR-001-clean-architecture.md) | Adopt Clean Architecture with CQRS |
| [ADR-002](adr/ADR-002-postgresql.md) | Use PostgreSQL 16 with EF Core 9 |
| [ADR-003](adr/ADR-003-tanstack-query.md) | Use TanStack Query for server state |
| [ADR-004](adr/ADR-004-testcontainers.md) | Use Testcontainers for integration tests |
| [ADR-005](adr/ADR-005-minimal-api.md) | Use Minimal API over MVC controllers |
