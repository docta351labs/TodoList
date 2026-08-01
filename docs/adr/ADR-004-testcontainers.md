# ADR-004 - Use Testcontainers for Integration Tests

**Date**: 2026-08-01  
**Status**: Accepted

## Context

Integration tests need to verify that EF Core mappings, migrations, and SQL queries work correctly against a real PostgreSQL database. Options: shared test database, SQLite in-memory, in-memory EF Core provider, Testcontainers.

## Decision

Use **Testcontainers.PostgreSql** to spin up a real, isolated PostgreSQL container per test class.

Each `IAsyncLifetime` test class starts a fresh container, applies migrations, and disposes the container after the tests complete.

## Consequences

**Positive:**
- Tests run against real PostgreSQL - no SQLite dialect discrepancies.
- Each test class gets an isolated database - no shared state between test runs.
- Migrations are validated as part of the test suite.
- Works in CI/CD pipelines with Docker available.

**Negative:**
- Tests are slower than in-memory alternatives (container startup ~2-4 seconds).
- Requires Docker to be running locally.

## Pattern

```csharp
public class TodoListRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    private AppDbContext _dbContext = null!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
            .UseSnakeCaseNamingConvention()
            .Options;
        _dbContext = new AppDbContext(options);
        await _dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _dbContainer.StopAsync();
    }

    [Fact]
    public async Task AddTodoList_ShouldPersistToDatabase()
    {
        // Arrange + Act + Assert ...
    }
}
```

## Alternatives Rejected

| Alternative | Reason rejected |
|-------------|----------------|
| EF Core In-Memory provider | Does not support SQL-specific features; migration testing impossible |
| SQLite | Different SQL dialect; some EF Core behaviors differ |
| Shared test DB | Flaky tests due to shared state; hard to parallelize |
