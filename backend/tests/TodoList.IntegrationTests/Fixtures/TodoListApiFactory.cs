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

    public async Task InitializeAsync()
    {
        await _dbFixture.InitializeAsync();

        if (!_dbFixture.IsContainerRunning)
        {
            _sqliteConnection = new SqliteConnection("DataSource=:memory:");
            await _sqliteConnection.OpenAsync();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_sqliteConnection)
                .UseSnakeCaseNamingConvention()
                .Options;

            using var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                d.ServiceType == typeof(DbContextOptions) ||
                d.ServiceType.Name.Contains("DbContextOptions")).ToList();

            foreach (var d in descriptors)
            {
                services.Remove(d);
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
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite(_sqliteConnection!)
                           .UseSnakeCaseNamingConvention();
                });
            }
        });
    }

    public new async Task DisposeAsync()
    {
        if (_sqliteConnection != null)
        {
            await _sqliteConnection.CloseAsync();
            await _sqliteConnection.DisposeAsync();
        }
        await _dbFixture.DisposeAsync();
        await base.DisposeAsync();
    }
}
