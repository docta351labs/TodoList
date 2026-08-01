using FluentAssertions;
using NSubstitute;
using TodoList.Application.Features.TodoItems.Commands.UpdateTodoItemStatus;
using TodoList.Application.Interfaces;
using TodoList.Domain.Aggregates;
using TodoList.Domain.Enums;

namespace TodoList.UnitTests.Application;

public class UpdateTodoItemStatusCommandHandlerTests
{
    private readonly ITodoListRepository _repository = Substitute.For<ITodoListRepository>();

    [Fact]
    public async Task UpdateTodoItemStatusCommandHandler_ValidTransition_ShouldCallRepositoryUpdateAsync()
    {
        // Arrange
        var list = TodoListAggregate.Create("Work", Guid.NewGuid());
        var item = list.AddItem("Task", null, Priority.Medium, null); // Pending

        _repository.GetByIdAsync(list.Id, Arg.Any<CancellationToken>()).Returns(list);

        var handler = new UpdateTodoItemStatusCommandHandler(_repository);
        var command = new UpdateTodoItemStatusCommand(list.Id, item.Id, Status.InProgress);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(Status.InProgress);
        await _repository.Received(1).UpdateAsync(list, Arg.Any<CancellationToken>());
    }
}
