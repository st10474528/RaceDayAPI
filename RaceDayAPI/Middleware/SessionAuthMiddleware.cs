using RaceDayAPI.Services;

namespace RaceDayAPI.Middleware;

public class SessionAuthMiddleware
{
    private readonly RequestDelegate _next;

    public SessionAuthMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ISessionService session)
    {
        var user = session.GetUser(context);
        if (user is not null)
        {
            context.Items["UserId"] = user.Value.UserId;
            context.Items["Role"] = user.Value.Role;
        }
        await _next(context);
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeRoleAttribute : Attribute
{
    public string[] Roles { get; }
    public AuthorizeRoleAttribute(params string[] roles) => Roles = roles;
}