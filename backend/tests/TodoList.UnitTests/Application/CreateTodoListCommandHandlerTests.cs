using FluentAssertions;
using NSubstitute;
using TodoList.Application.Features.TodoLists.Commands.CreateTodoList;
using TodoList.Application.Interfaces;
using TodoList.Domain.Aggregates;

namespace TodoList.UnitTests.Application;

public class CreateTodoListCommandHandlerTests
{
    private readonly ITodoListRepository _repository = Substitute.For<ITodoListRepository>();

    [Fact]
    public async Task CreateTodoListCommandHandler_ValidCommand_ShouldReturnTodoListDto()
    {
        // Arrange
        var handler = new CreateTodoListCommandHandler(_repository);
        var command = new CreateTodoListCommand("Work Projects", Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Work Projects");
        result.OwnerId.Should().Be(command.OwnerId);
    }

    [Fact]
    public async Task CreateTodoListCommandHandler_ValidCommand_ShouldCallRepositoryAddAsync()
    {
        // Arrange
        var handler = new CreateTodoListCommandHandler(_repository);
        var command = new CreateTodoListCommand("Personal Tasks", Guid.NewGuid());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await _repository.Received(1).AddAsync(Arg.Is<TodoListAggregate>(l => l != null && l.Title == "Personal Tasks"), Arg.Any<CancellationToken>());
    }
}
