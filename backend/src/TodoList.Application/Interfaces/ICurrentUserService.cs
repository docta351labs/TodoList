namespace TodoList.Application.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
}
