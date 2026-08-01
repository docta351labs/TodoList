using FluentAssertions;
using TodoList.Domain.Aggregates;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Domain.Events;
using TodoList.Domain.Exceptions;

namespace TodoList.UnitTests.Domain;

public class TodoListTests
{
    [Fact]
    public void Create_WithValidTitle_ShouldRaiseTodoListCreatedEvent()
    {
        // Arrange
        var title = "Work Projects";
        var ownerId = Guid.NewGuid();

        // Act
        var list = TodoListAggregate.Create(title, ownerId);

        // Assert
        list.Should().NotBeNull();
        list.Title.Should().Be(title);
        list.OwnerId.Should().Be(ownerId);
        list.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TodoListCreatedEvent>()
            .Which.Should().BeEquivalentTo(new TodoListCreatedEvent(list.Id, title, ownerId));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyTitle_ShouldThrowDomainException(string invalidTitle)
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Act
        Action act = () => TodoListAggregate.Create(invalidTitle, ownerId);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Title*");
    }

    [Fact]
    public void Create_WithTitleExceeding100Chars_ShouldThrowDomainException()
    {
        // Arrange
        var longTitle = new string('A', 101);
        var ownerId = Guid.NewGuid();

        // Act
        Action act = () => TodoListAggregate.Create(longTitle, ownerId);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*100*");
    }

    [Fact]
    public void AddItem_WithValidTitle_ShouldRaiseTodoItemAddedEvent()
    {
        // Arrange
        var list = TodoListAggregate.Create("Work", Guid.NewGuid());
        list.ClearDomainEvents();

        // Act
        var item = list.AddItem("Buy Groceries", "Milk and Eggs", Priority.High, DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        // Assert
        list.Items.Should().ContainSingle(i => i.Id == item.Id);
        list.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TodoItemAddedEvent>()
            .Which.Should().BeEquivalentTo(new TodoItemAddedEvent(list.Id, item.Id));
    }

    [Fact]
    public void AddItem_WhenListHas500Items_ShouldThrowItemLimitExceededException()
    {
        // Arrange
        var list = TodoListAggregate.Create("Huge List", Guid.NewGuid());
        for (int i = 0; i < 500; i++)
        {
            list.AddItem($"Item {i + 1}", null, Priority.Low, null);
        }

        // Act
        Action act = () => list.AddItem("Overflow Item", null, Priority.Low, null);

        // Assert
        act.Should().Throw<ItemLimitExceededException>();
    }

    [Fact]
    public void DeleteItem_WithExistingItem_ShouldRaiseTodoItemDeletedEvent()
    {
        // Arrange
        var list = TodoListAggregate.Create("My List", Guid.NewGuid());
        var item = list.AddItem("Task to Delete", null, Priority.Medium, null);
        list.ClearDomainEvents();

        // Act
        list.DeleteItem(item.Id);

        // Assert
        list.Items.Should().BeEmpty();
        list.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TodoItemDeletedEvent>()
            .Which.Should().BeEquivalentTo(new TodoItemDeletedEvent(list.Id, item.Id));
    }
}
