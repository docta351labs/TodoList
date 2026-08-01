using FluentValidation;
using MediatR;
using TodoList.Application.Common;
using TodoList.Application.DTOs;
using TodoList.Application.Exceptions;
using TodoList.Application.Interfaces;
using TodoList.Domain.Aggregates;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Application.Features.TodoItems.Commands.UpdateTodoItem;

public record UpdateTodoItemCommand(
    Guid ListId,
    Guid ItemId,
    string Title,
    string? Description,
    Priority Priority,
    DateOnly? DueDate) : ICommand<TodoItemDto>;

public class UpdateTodoItemValidator : AbstractValidator<UpdateTodoItemCommand>
{
    public UpdateTodoItemValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(v => v.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
    }
}

public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand, TodoItemDto>
{
    private readonly ITodoListRepository _repository;

    public UpdateTodoItemCommandHandler(ITodoListRepository repository)
    {
        _repository = repository;
    }

    public async Task<TodoItemDto> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetByIdAsync(request.ListId, cancellationToken);
        if (list == null)
        {
            throw new NotFoundException(nameof(TodoListAggregate), request.ListId);
        }

        var item = list.Items.FirstOrDefault(i => i.Id == request.ItemId);
        if (item == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.ItemId);
        }

        item.Update(request.Title, request.Description, request.Priority, request.DueDate);
        await _repository.UpdateAsync(list, cancellationToken);

        return TodoItemDto.FromEntity(item);
    }
}
