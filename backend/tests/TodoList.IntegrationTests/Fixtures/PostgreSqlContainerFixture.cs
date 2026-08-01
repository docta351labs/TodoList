using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using TodoList.Infrastructure.Persistence;

namespace TodoList.IntegrationTests.Fixtures;

public class PostgreSqlContainerFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;

    public bool IsContainerRunning => _container != null;
    public string ConnectionString => _container?.GetConnectionString() ?? string.Empty;

    public async Task InitializeAsync()
    {
        if (OperatingSystem.IsWindows())
        {
            try
            {
                using var pipe = new System.IO.Pipes.NamedPipeClientStream(".", "docker_engine", System.IO.Pipes.PipeDirection.InOut);
                pipe.Connect(500);
            }
            catch
            {
                _container = null;
                return;
            }
        }

        try
        {
            _container = new PostgreSqlBuilder("postgres:16-alpine")
                .WithDatabase("todolist_test")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

            await _container.StartAsync();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(ConnectionString)
                          .UseSnakeCaseNamingConvention();

            using var dbContext = new AppDbContext(optionsBuilder.Options);
            await dbContext.Database.MigrateAsync();
        }
        catch
        {
            _container = null;
        }
    }

    public async Task DisposeAsync()
    {
        if (_container != null)
        {
            await _container.DisposeAsync();
        }
    }
}
