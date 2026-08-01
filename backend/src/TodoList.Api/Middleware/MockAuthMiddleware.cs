using System.Security.Claims;

namespace TodoList.Api.Middleware;

public class MockAuthMiddleware
{
    private readonly RequestDelegate _next;
    public static readonly Guid MockUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public MockAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // TODO: Replace with JWT Bearer auth when Identity Provider is integrated
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, MockUserId.ToString()),
            new Claim("sub", MockUserId.ToString()),
            new Claim("owner_id", MockUserId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "MockAuth");
        context.User = new ClaimsPrincipal(identity);

        await _next(context);
    }
}
