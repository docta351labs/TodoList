# TodoList — Project README

## Overview

A full-stack Todo List application built with **ASP.NET Core 9** (backend) and **React 19** (frontend), backed by **PostgreSQL 16**.

- SOLID principles + Clean Architecture + CQRS
- TDD with xUnit, FluentAssertions, and Testcontainers
- React 19 + TanStack Query v5 + TypeScript 5 (strict)

---

## Prerequisites

| Tool           | Minimum Version    |
| ---------------| -------------------|
| .NET SDK       | 9.0                |
| Node.js        | 22 LTS             |
| Docker Desktop | 27+                |
| PostgreSQL     | 16 (or via Docker) |

---

## Quick Start

### 1. Clone the repository

```bash
git clone <repo-url>
cd TodoList
```

### 2. Configure backend secrets

```bash
cd backend/src/TodoList.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Port=5432;Database=todolist;Username=postgres;Password=yourpassword"
dotnet user-secrets set "Jwt:Secret" "your-256-bit-secret-here"
```

### 3. Start PostgreSQL (Docker)

```bash
docker run -d \
  --name todolist-db \
  -e POSTGRES_DB=todolist \
  -e POSTGRES_PASSWORD=yourpassword \
  -p 5432:5432 \
  postgres:16-alpine
```

### 4. Apply migrations

```bash
cd backend/src/TodoList.Api
dotnet ef database update
```

### 5. Run the backend API

```bash
dotnet run
# API available at: https://localhost:7001
# Scalar UI:        https://localhost:7001/scalar
# Health check:     https://localhost:7001/health
```

### 6. Run the frontend

```bash
cd frontend
npm install
npm run dev
# Frontend available at: http://localhost:5173
```

---

## Running Tests

### Backend — unit tests

```bash
cd backend
dotnet test tests/TodoList.UnitTests
```

### Backend — integration tests (requires Docker)

```bash
dotnet test tests/TodoList.IntegrationTests
```

### All backend tests

```bash
dotnet test
```

### Frontend — component tests

```bash
cd frontend
npm run test
```

### Frontend — E2E (requires both servers running)

```bash
npm run test:e2e
```

---

## Project Structure

```
TodoList/
├── backend/
│   ├── src/
│   │   ├── TodoList.Api/            # Web API host (minimal API)
│   │   ├── TodoList.Application/    # CQRS handlers, validators, DTOs
│   │   ├── TodoList.Domain/         # Entities, domain events, value objects
│   │   └── TodoList.Infrastructure/ # EF Core, migrations, repositories
│   └── tests/
│       ├── TodoList.UnitTests/
│       └── TodoList.IntegrationTests/
├── frontend/
│   └── src/
│       ├── api/         # Axios client
│       ├── components/  # Shared UI components
│       ├── features/    # Feature modules (todos/)
│       ├── pages/       # Route pages
│       └── types/       # TypeScript types
├── docs/
│   ├── adr/             # Architecture Decision Records
│   ├── design.md        # Architecture & domain design
│   └── dependencies.md  # NuGet & npm package list
├── AGENTS.md            # AI coding guardrails
└── README.md
```

---

## Key Technologies

### Backend

| Package | Purpose |
|---------|---------|
| MediatR | CQRS pipeline |
| FluentValidation | Input validation |
| EF Core 9 (Npgsql) | ORM + PostgreSQL driver |
| Scalar.AspNetCore | OpenAPI docs UI |
| Serilog | Structured logging |
| OpenTelemetry | Distributed tracing |
| NSubstitute | Test mocking |
| Testcontainers | PostgreSQL for integration tests |
| NetArchTest | Architecture rule tests |
| FluentAssertions | Assertion library |

### Frontend

| Package | Purpose |
|---------|---------|
| React 19 | UI framework |
| React Router v7 | Client-side routing |
| TanStack Query v5 | Server state management |
| Zustand | Client UI state |
| Axios | HTTP client |
| Zod | Runtime schema validation |
| MSW | API mocking for tests |
| Vitest + RTL | Unit & component testing |
| Playwright | E2E testing |

---

## Contributing

1. Read `AGENTS.md` before writing any code.
2. Follow the **Red → Green → Refactor** TDD cycle.
3. Use Conventional Commits for commit messages.
4. Open a PR — all checks must pass before merging.

---

## License

MIT
