using FluentAssertions;
using NSubstitute;
using TodoList.Application.Exceptions;
using TodoList.Application.Features.TodoItems.Commands.AddTodoItem;
using TodoList.Application.Interfaces;
using TodoList.Domain.Aggregates;
using TodoList.Domain.Enums;

namespace TodoList.UnitTests.Application;

public class AddTodoItemCommandHandlerTests
{
    private readonly ITodoListRepository _repository = Substitute.For<ITodoListRepository>();

    [Fact]
    public async Task AddTodoItemCommandHandler_ValidCommand_ShouldReturnTodoItemDto()
    {
        // Arrange
        var list = TodoListAggregate.Create("Work", Guid.NewGuid());
        _repository.GetByIdAsync(list.Id, Arg.Any<CancellationToken>()).Returns(list);

        var handler = new AddTodoItemCommandHandler(_repository);
        var command = new AddTodoItemCommand(list.Id, "Buy Laptop", "MacBook Pro", Priority.High, DateOnly.FromDateTime(DateTime.Today.AddDays(7)));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Buy Laptop");
        result.Priority.Should().Be(Priority.High);
    }

    [Fact]
    public async Task AddTodoItemCommandHandler_ListNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var missingId = Guid.NewGuid();
        _repository.GetByIdAsync(missingId, Arg.Any<CancellationToken>()).Returns((TodoListAggregate?)null);

        var handler = new AddTodoItemCommandHandler(_repository);
        var command = new AddTodoItemCommand(missingId, "Item", null, Priority.Low, null);

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
