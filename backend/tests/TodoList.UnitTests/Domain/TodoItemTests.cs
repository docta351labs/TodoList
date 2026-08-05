using FluentAssertions;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Domain.Events;
using TodoList.Domain.Exceptions;

namespace TodoList.UnitTests.Domain;

public class TodoItemTests
{
    [Fact]
    public void Start_WhenStatusIsPending_ShouldTransitionToInProgress()
    {
        // Arrange
        var item = TodoItem.Create("Write Docs", "Write user guide", Priority.Medium, null);

        // Act
        item.Start();

        // Assert
        item.Status.Should().Be(Status.InProgress);
    }

    [Fact]
    public void Start_WhenStatusIsNotPending_ShouldThrowInvalidStatusTransitionException()
    {
        // Arrange
        var item = TodoItem.Create("Write Code", null, Priority.High, null);
        item.Start(); // now InProgress

        // Act
        Action act = () => item.Start();

        // Assert
        act.Should().Throw<InvalidStatusTransitionException>();
    }

    [Fact]
    public void Complete_WhenStatusIsInProgress_ShouldSetCompletedAt()
    {
        // Arrange
        var item = TodoItem.Create("Deploy", null, Priority.High, null);
        item.Start();

        // Act
        item.Complete();

        // Assert
        item.Status.Should().Be(Status.Done);
        item.CompletedAt.Should().NotBeNull();
        item.CompletedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Complete_WhenStatusIsNotInProgress_ShouldThrowInvalidStatusTransitionException()
    {
        // Arrange
        var item = TodoItem.Create("Design", null, Priority.Low, null); // Pending

        // Act
        Action act = () => item.Complete();

        // Assert
        act.Should().Throw<InvalidStatusTransitionException>();
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateProperties()
    {
        // Arrange
        var item = TodoItem.Create("Write Docs", "Draft", Priority.Low, null);
        var newDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));

        // Act
        item.Update("Write Docs v2", "Final draft", Priority.High, newDueDate);

        // Assert
        item.Title.Should().Be("Write Docs v2");
        item.Description.Should().Be("Final draft");
        item.Priority.Should().Be(Priority.High);
        item.DueDate.Should().Be(newDueDate);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithEmptyTitle_ShouldThrowDomainException(string title)
    {
        // Arrange
        var item = TodoItem.Create("Write Docs", null, Priority.Medium, null);

        // Act
        Action act = () => item.Update(title, null, Priority.Medium, null);

        // Assert
        act.Should().Throw<DomainException>();
    }
}
