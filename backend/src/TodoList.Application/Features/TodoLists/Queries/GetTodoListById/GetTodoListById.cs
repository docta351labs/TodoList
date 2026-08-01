using MediatR;
using TodoList.Application.Common;
using TodoList.Application.DTOs;
using TodoList.Application.Exceptions;
using TodoList.Application.Interfaces;
using TodoList.Domain.Aggregates;

namespace TodoList.Application.Features.TodoLists.Queries.GetTodoListById;

public record GetTodoListByIdQuery(Guid ListId) : IQuery<TodoListDto>;

public class GetTodoListByIdQueryHandler : IRequestHandler<GetTodoListByIdQuery, TodoListDto>
{
    private readonly ITodoListRepository _repository;

    public GetTodoListByIdQueryHandler(ITodoListRepository repository)
    {
        _repository = repository;
    }

    public async Task<TodoListDto> Handle(GetTodoListByIdQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetByIdAsync(request.ListId, cancellationToken);
        if (list == null)
        {
            throw new NotFoundException(nameof(TodoListAggregate), request.ListId);
        }

        return TodoListDto.FromEntity(list);
    }
}
