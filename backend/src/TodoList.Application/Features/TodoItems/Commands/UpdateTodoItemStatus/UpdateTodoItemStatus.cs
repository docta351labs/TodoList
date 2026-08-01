using FluentValidation;
using MediatR;
using TodoList.Application.Common;
using TodoList.Application.DTOs;
using TodoList.Application.Exceptions;
using TodoList.Application.Interfaces;
using TodoList.Domain.Aggregates;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Application.Features.TodoItems.Commands.UpdateTodoItemStatus;

public record UpdateTodoItemStatusCommand(
    Guid ListId,
    Guid ItemId,
    Status NewStatus) : ICommand<TodoItemDto>;

public class UpdateTodoItemStatusValidator : AbstractValidator<UpdateTodoItemStatusCommand>
{
    public UpdateTodoItemStatusValidator()
    {
        RuleFor(v => v.NewStatus)
            .IsInEnum().WithMessage("Invalid status value.");
    }
}

public class UpdateTodoItemStatusCommandHandler : IRequestHandler<UpdateTodoItemStatusCommand, TodoItemDto>
{
    private readonly ITodoListRepository _repository;

    public UpdateTodoItemStatusCommandHandler(ITodoListRepository repository)
    {
        _repository = repository;
    }

    public async Task<TodoItemDto> Handle(UpdateTodoItemStatusCommand request, CancellationToken cancellationToken)
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

        if (request.NewStatus == Status.InProgress)
        {
            item.Start();
        }
        else if (request.NewStatus == Status.Done)
        {
            item.Complete();
        }

        await _repository.UpdateAsync(list, cancellationToken);
        return TodoItemDto.FromEntity(item);
    }
}
