using FluentAssertions;
using NSubstitute;
using TodoList.Application.Features.TodoLists.Commands.DeleteTodoList;
using TodoList.Application.Interfaces;
using TodoList.Domain.Aggregates;

namespace TodoList.UnitTests.Application;

public class DeleteTodoListCommandHandlerTests
{
    private readonly ITodoListRepository _repository = Substitute.For<ITodoListRepository>();

    [Fact]
    public async Task DeleteTodoListCommandHandler_ValidId_ShouldCallRepositoryDeleteAsync()
    {
        // Arrange
        var list = TodoListAggregate.Create("List to delete", Guid.NewGuid());
        _repository.GetByIdAsync(list.Id, Arg.Any<CancellationToken>()).Returns(list);

        var handler = new DeleteTodoListCommandHandler(_repository);
        var command = new DeleteTodoListCommand(list.Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await _repository.Received(1).DeleteAsync(list.Id, Arg.Any<CancellationToken>());
    }
}
