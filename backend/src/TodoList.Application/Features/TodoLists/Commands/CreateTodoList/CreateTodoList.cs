using FluentValidation;
using MediatR;
using TodoList.Application.Common;
using TodoList.Application.DTOs;
using TodoList.Application.Interfaces;
using TodoList.Domain.Aggregates;

namespace TodoList.Application.Features.TodoLists.Commands.CreateTodoList;

public record CreateTodoListCommand(string Title, Guid OwnerId) : ICommand<TodoListDto>;

public class CreateTodoListValidator : AbstractValidator<CreateTodoListCommand>
{
    public CreateTodoListValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
    }
}

public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, TodoListDto>
{
    private readonly ITodoListRepository _repository;

    public CreateTodoListCommandHandler(ITodoListRepository repository)
    {
        _repository = repository;
    }

    public async Task<TodoListDto> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var list = TodoListAggregate.Create(request.Title, request.OwnerId);
        await _repository.AddAsync(list, cancellationToken);
        return TodoListDto.FromEntity(list);
    }
}
