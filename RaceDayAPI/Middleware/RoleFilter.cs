using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDayAPI.Middleware;

public class RoleFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var attr = context.ActionDescriptor.EndpointMetadata
            .OfType<AuthorizeRoleAttribute>()
            .FirstOrDefault();

        if (attr is null) return;

        var userId = context.HttpContext.Items["UserId"];
        var role = context.HttpContext.Items["Role"] as string;

        if (userId is null)
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Not authenticated." });
            return;
        }

        if (!attr.Roles.Contains(role))
        {
            context.Result = new ObjectResult(new { message = "Forbidden: insufficient role." })
            { StatusCode = StatusCodes.Status403Forbidden };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
