using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Infrastructure.Persistence;

namespace TodoList.IntegrationTests.Fixtures;

public class TodoListApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainerFixture _dbFixture = new();
    private SqliteConnection? _sqliteConnection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            if (_dbFixture.IsContainerRunning)
            {
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(_dbFixture.ConnectionString)
                           .UseSnakeCaseNamingConvention();
                });
            }
            else
            {
                _sqliteConnection = new SqliteConnection("DataSource=:memory:");
                _sqliteConnection.Open();

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite(_sqliteConnection)
                           .UseSnakeCaseNamingConvention();
                });

                // Ensure schema is created for SQLite in-memory
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
            }
        });
    }

    public async Task InitializeAsync()
    {
        await _dbFixture.InitializeAsync();
    }

    public new async Task DisposeAsync()
    {
        _sqliteConnection?.Close();
        _sqliteConnection?.Dispose();
        await _dbFixture.DisposeAsync();
        await base.DisposeAsync();
    }
}
