using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Persistence.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("todo_items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .ValueGeneratedNever();

        builder.Property(i => i.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.Description)
            .IsRequired(false);

        builder.Property(i => i.Priority)
            .IsRequired();

        builder.Property(i => i.Status)
            .IsRequired();

        builder.Property(i => i.DueDate)
            .IsRequired(false);

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.CompletedAt)
            .IsRequired(false);

        builder.Property<Guid>("TodoListId")
            .IsRequired();

        builder.HasIndex("TodoListId")
            .HasDatabaseName("idx_todo_items_list_id");
    }
}
