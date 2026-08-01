using MediatR;
using TodoList.Application.Common;
using TodoList.Application.Exceptions;
using TodoList.Application.Interfaces;
using TodoList.Domain.Aggregates;

namespace TodoList.Application.Features.TodoItems.Commands.DeleteTodoItem;

public record DeleteTodoItemCommand(Guid ListId, Guid ItemId) : ICommand<Unit>;

public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand, Unit>
{
    private readonly ITodoListRepository _repository;

    public DeleteTodoItemCommandHandler(ITodoListRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetByIdAsync(request.ListId, cancellationToken);
        if (list == null)
        {
            throw new NotFoundException(nameof(TodoListAggregate), request.ListId);
        }

        list.DeleteItem(request.ItemId);
        await _repository.UpdateAsync(list, cancellationToken);

        return Unit.Value;
    }
}
