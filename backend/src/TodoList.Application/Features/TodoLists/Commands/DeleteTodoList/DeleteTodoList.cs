using MediatR;
using TodoList.Application.Common;
using TodoList.Application.Exceptions;
using TodoList.Application.Interfaces;
using TodoList.Domain.Aggregates;

namespace TodoList.Application.Features.TodoLists.Commands.DeleteTodoList;

public record DeleteTodoListCommand(Guid ListId) : ICommand<Unit>;

public class DeleteTodoListCommandHandler : IRequestHandler<DeleteTodoListCommand, Unit>
{
    private readonly ITodoListRepository _repository;

    public DeleteTodoListCommandHandler(ITodoListRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetByIdAsync(request.ListId, cancellationToken);
        if (list == null)
        {
            throw new NotFoundException(nameof(TodoListAggregate), request.ListId);
        }

        await _repository.DeleteAsync(request.ListId, cancellationToken);
        return Unit.Value;
    }
}
