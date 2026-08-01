# Dependencies Reference

> Keep this file updated whenever a package is added or upgraded.

---

## Backend (NuGet Packages)

### TodoList.Api

| Package | Version | Purpose |
|---------|---------|---------|
| `Scalar.AspNetCore` | 2.x | OpenAPI / Scalar UI |
| `Microsoft.AspNetCore.OpenApi` | 9.x | OpenAPI document generation |
| `Serilog.AspNetCore` | 8.x | Structured logging |
| `Serilog.Sinks.Console` | 5.x | Console sink |
| `Serilog.Sinks.Seq` | 7.x | Seq sink (optional, for local dev) |
| `OpenTelemetry.Extensions.Hosting` | 1.x | Telemetry host integration |
| `OpenTelemetry.Instrumentation.AspNetCore` | 1.x | HTTP tracing |
| `OpenTelemetry.Instrumentation.Http` | 1.x | HttpClient tracing |
| `OpenTelemetry.Exporter.Console` | 1.x | Console exporter (dev) |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 9.x | JWT authentication |

### TodoList.Application

| Package | Version | Purpose |
|---------|---------|---------|
| `MediatR` | 12.x | CQRS / mediator pattern |
| `FluentValidation` | 11.x | Command/query input validation |
| `FluentValidation.DependencyInjectionExtensions` | 11.x | DI registration helpers |

### TodoList.Domain

| Package | Version | Purpose |
|---------|---------|---------|
| *(none)* | - | Domain is pure C# with no external deps |

### TodoList.Infrastructure

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.EntityFrameworkCore` | 9.x | ORM core |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 9.x | PostgreSQL driver |
| `EFCore.NamingConventions` | 8.x | snake_case naming |
| `Microsoft.EntityFrameworkCore.Design` | 9.x | Migrations tooling |

### TodoList.UnitTests

| Package | Version | Purpose |
|---------|---------|---------|
| `xunit` | 2.x | Test framework |
| `xunit.runner.visualstudio` | 2.x | VS test runner |
| `FluentAssertions` | 7.x | Assertion library |
| `NSubstitute` | 5.x | Mocking |
| `Microsoft.NET.Test.Sdk` | 17.x | Test host |

### TodoList.IntegrationTests

| Package | Version | Purpose |
|---------|---------|---------|
| `xunit` | 2.x | Test framework |
| `xunit.runner.visualstudio` | 2.x | VS test runner |
| `FluentAssertions` | 7.x | Assertion library |
| `Microsoft.AspNetCore.Mvc.Testing` | 9.x | WebApplicationFactory |
| `Testcontainers.PostgreSql` | 3.x | PostgreSQL in Docker for tests |
| `NetArchTest.Rules` | 1.x | Architecture layer tests |
| `Microsoft.NET.Test.Sdk` | 17.x | Test host |

---

## Frontend (npm Packages)

### Runtime Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `react` | ^19.0.0 | UI framework |
| `react-dom` | ^19.0.0 | DOM renderer |
| `react-router` | ^7.0.0 | Client-side routing |
| `@tanstack/react-query` | ^5.0.0 | Server state management |
| `@tanstack/react-query-devtools` | ^5.0.0 | Query devtools |
| `axios` | ^1.7.0 | HTTP client |
| `zod` | ^3.23.0 | Runtime schema validation |
| `zustand` | ^5.0.0 | Client UI state |

### Dev Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `typescript` | ^5.5.0 | Type checker |
| `vite` | ^6.0.0 | Build tool |
| `@vitejs/plugin-react` | ^4.0.0 | React fast refresh |
| `vitest` | ^2.0.0 | Unit/component test runner |
| `@vitest/ui` | ^2.0.0 | Vitest browser UI |
| `@testing-library/react` | ^16.0.0 | Component testing |
| `@testing-library/user-event` | ^14.0.0 | User interaction simulation |
| `@testing-library/jest-dom` | ^6.0.0 | DOM matchers |
| `msw` | ^2.0.0 | API mocking (service worker) |
| `playwright` | ^1.45.0 | E2E testing |
| `@playwright/test` | ^1.45.0 | Playwright test runner |
| `eslint` | ^9.0.0 | Linter |
| `@typescript-eslint/parser` | ^8.0.0 | TS ESLint parser |
| `@typescript-eslint/eslint-plugin` | ^8.0.0 | TS ESLint rules |
| `prettier` | ^3.0.0 | Code formatter |
