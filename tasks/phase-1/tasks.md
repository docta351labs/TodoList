# Phase 1 — Backend: Project Scaffolding

## Tasks

- [ ] **1.1** Create `TodoList.Domain` class library: `dotnet new classlib -n TodoList.Domain -o backend/src/TodoList.Domain`
- [ ] **1.2** Create `TodoList.Application` class library: `dotnet new classlib -n TodoList.Application -o backend/src/TodoList.Application`
- [ ] **1.3** Create `TodoList.Infrastructure` class library: `dotnet new classlib -n TodoList.Infrastructure -o backend/src/TodoList.Infrastructure`
- [ ] **1.4** Create `TodoList.Api` web API project: `dotnet new webapi -n TodoList.Api -o backend/src/TodoList.Api`
- [ ] **1.5** Create `TodoList.UnitTests` xUnit project: `dotnet new xunit -n TodoList.UnitTests -o backend/tests/TodoList.UnitTests`
- [ ] **1.6** Create `TodoList.IntegrationTests` xUnit project: `dotnet new xunit -n TodoList.IntegrationTests -o backend/tests/TodoList.IntegrationTests`
- [ ] **1.7** Add all projects to the solution file
- [ ] **1.8** Wire project references in the correct direction:
  - `Application` → references `Domain`
  - `Infrastructure` → references `Application` + `Domain`
  - `Api` → references `Infrastructure` + `Application`
  - `UnitTests` → references `Domain` + `Application`
  - `IntegrationTests` → references `Api` + `Infrastructure`
- [ ] **1.9** Enable `<Nullable>enable</Nullable>` and `<ImplicitUsings>enable</ImplicitUsings>` in every `.csproj`
- [ ] **1.10** Install NuGet packages per [`docs/dependencies.md`](../../docs/dependencies.md):
  - **Api**: `Scalar.AspNetCore`, `Microsoft.AspNetCore.OpenApi`, `Serilog.AspNetCore`, `Serilog.Sinks.Console`, `Serilog.Sinks.Seq`, `OpenTelemetry.*`, `Microsoft.AspNetCore.Authentication.JwtBearer`
  - **Application**: `MediatR`, `FluentValidation`, `FluentValidation.DependencyInjectionExtensions`
  - **Domain**: *(none)*
  - **Infrastructure**: `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`, `EFCore.NamingConventions`, `Microsoft.EntityFrameworkCore.Design`
  - **UnitTests**: `xunit`, `xunit.runner.visualstudio`, `FluentAssertions`, `NSubstitute`, `Microsoft.NET.Test.Sdk`
  - **IntegrationTests**: `xunit`, `xunit.runner.visualstudio`, `FluentAssertions`, `Microsoft.AspNetCore.Mvc.Testing`, `Testcontainers.PostgreSql`, `NetArchTest.Rules`, `Microsoft.NET.Test.Sdk`
- [ ] **1.11** Verify `dotnet build` produces **zero warnings** and **zero errors**

## Notes

- Layer dependency rule (AGENTS.md §3.1): `Domain ← Application ← Infrastructure ← Api`
- No business logic in `Api` or `Infrastructure`.
- No persistence details in `Domain` or `Application`.
