using FluentAssertions;
using TodoList.Application.Features.TodoLists.Commands.CreateTodoList;

namespace TodoList.UnitTests.Application;

public class CreateTodoListValidatorTests
{
    private readonly CreateTodoListValidator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateTodoListValidator_EmptyTitle_ShouldHaveValidationError(string emptyTitle)
    {
        // Arrange
        var command = new CreateTodoListCommand(emptyTitle, Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTodoListCommand.Title));
    }

    [Fact]
    public void CreateTodoListValidator_TitleExceeds100Chars_ShouldHaveValidationError()
    {
        // Arrange
        var longTitle = new string('X', 101);
        var command = new CreateTodoListCommand(longTitle, Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTodoListCommand.Title));
    }
}
