using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoList.Application.Interfaces;
using TodoList.Domain.Aggregates;
using TodoList.Domain.Common;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    private readonly IPublisher? _publisher;

    public DbSet<TodoListAggregate> TodoLists => Set<TodoListAggregate>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public AppDbContext(DbContextOptions<AppDbContext> options, IPublisher? publisher = null)
        : base(options)
    {
        _publisher = publisher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Count != 0)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        if (_publisher != null)
        {
            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
        }
    }
}
