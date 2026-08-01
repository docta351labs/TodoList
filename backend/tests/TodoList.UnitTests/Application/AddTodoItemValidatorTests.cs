using FluentAssertions;
using TodoList.Application.Features.TodoItems.Commands.AddTodoItem;
using TodoList.Domain.Enums;

namespace TodoList.UnitTests.Application;

public class AddTodoItemValidatorTests
{
    private readonly AddTodoItemValidator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddTodoItemValidator_EmptyTitle_ShouldHaveValidationError(string emptyTitle)
    {
        // Arrange
        var command = new AddTodoItemCommand(Guid.NewGuid(), emptyTitle, null, Priority.Medium, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddTodoItemCommand.Title));
    }

    [Fact]
    public void AddTodoItemValidator_DescriptionExceeds1000Chars_ShouldHaveValidationError()
    {
        // Arrange
        var longDescription = new string('D', 1001);
        var command = new AddTodoItemCommand(Guid.NewGuid(), "Valid Title", longDescription, Priority.Medium, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddTodoItemCommand.Description));
    }
}
