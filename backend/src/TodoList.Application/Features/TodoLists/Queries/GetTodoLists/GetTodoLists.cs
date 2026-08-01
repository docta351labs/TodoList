using MediatR;
using TodoList.Application.Common;
using TodoList.Application.DTOs;
using TodoList.Application.Interfaces;

namespace TodoList.Application.Features.TodoLists.Queries.GetTodoLists;

public record GetTodoListsQuery(Guid OwnerId) : IQuery<IReadOnlyList<TodoListSummaryDto>>;

public class GetTodoListsQueryHandler : IRequestHandler<GetTodoListsQuery, IReadOnlyList<TodoListSummaryDto>>
{
    private readonly ITodoListRepository _repository;

    public GetTodoListsQueryHandler(ITodoListRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TodoListSummaryDto>> Handle(GetTodoListsQuery request, CancellationToken cancellationToken)
    {
        var lists = await _repository.GetAllByOwnerIdAsync(request.OwnerId, cancellationToken);
        return lists.Select(TodoListSummaryDto.FromEntity).ToList();
    }
}
