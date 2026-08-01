using MediatR;

namespace TodoList.Application.Common;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

public interface ICommand : IRequest
{
}

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
