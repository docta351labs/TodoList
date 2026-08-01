# ADR-002 - Use PostgreSQL 16 with EF Core 9

**Date**: 2026-08-01  
**Status**: Accepted

## Context

We need a relational database for persisting todo lists and items. Options considered: PostgreSQL, SQL Server, SQLite, MySQL.

## Decision

Use **PostgreSQL 16** as the database engine and **EF Core 9 (Npgsql)** as the ORM.

- `EFCore.NamingConventions` enforces `snake_case` column/table names (PostgreSQL convention).
- Migrations are managed by EF Core and version-controlled in `TodoList.Infrastructure/Migrations/`.
- Testcontainers spins up a real PostgreSQL container for integration tests, guaranteeing test fidelity.

## Consequences

**Positive:**
- PostgreSQL is open-source, production-proven, and supports advanced features (JSONB, full-text search, window functions) for future needs.
- EF Core 9 has first-class PostgreSQL support via Npgsql.
- `snake_case` naming avoids quoting issues in raw SQL and aligns with PostgreSQL conventions.

**Negative:**
- Requires Docker for local development (unless PostgreSQL is installed locally).
- EF Core migrations need careful management in team environments.

## Conventions

```csharp
// Program.cs / DI setup
services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention());
```

## Alternatives Rejected

| Alternative | Reason rejected |
|-------------|----------------|
| SQL Server | Licensing costs; less idiomatic for open-source projects |
| SQLite | Not suitable for production concurrent workloads |
| MySQL | PostgreSQL preferred for feature richness and tooling |
