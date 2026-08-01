using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Application.Interfaces;
using TodoList.Infrastructure.Persistence;
using TodoList.Infrastructure.Repositories;
using TodoList.Infrastructure.Services;

namespace TodoList.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=todolist;Username=postgres;Password=changeme";

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString)
                   .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<ITodoListRepository, TodoListRepository>();
        services.AddScoped<ICurrentUserService, MockCurrentUserService>();

        return services;
    }
}
