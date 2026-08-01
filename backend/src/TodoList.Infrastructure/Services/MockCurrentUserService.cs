using TodoList.Application.Interfaces;

namespace TodoList.Infrastructure.Services;

public class MockCurrentUserService : ICurrentUserService
{
    public Guid UserId => Guid.Parse("00000000-0000-0000-0000-000000000001");
}
