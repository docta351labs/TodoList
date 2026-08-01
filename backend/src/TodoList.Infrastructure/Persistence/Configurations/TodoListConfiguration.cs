using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.Domain.Aggregates;

namespace TodoList.Infrastructure.Persistence.Configurations;

public class TodoListConfiguration : IEntityTypeConfiguration<TodoListAggregate>
{
    public void Configure(EntityTypeBuilder<TodoListAggregate> builder)
    {
        builder.ToTable("todo_lists");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .ValueGeneratedNever();

        builder.Property(l => l.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(l => l.OwnerId)
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.HasMany(l => l.Items)
            .WithOne()
            .HasForeignKey("TodoListId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(l => l.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(l => l.OwnerId)
            .HasDatabaseName("idx_todo_lists_owner_id");
    }
}
