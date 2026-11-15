using CompuCare.Platform.Iam.Domain.Model.Aggregates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
///     Rejects a request with 401 unless <see cref="Components.RequestAuthorizationMiddleware" /> already placed
///     a signed-in <see cref="User" /> on <c>HttpContext.Items["User"]</c>, or the action allows anonymous access.
///     When roles are given, a signed-in user with any other role is rejected with 403.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AuthorizeAttribute(params string[] roles) : Attribute, IAuthorizationFilter
{
    public IReadOnlyCollection<string> Roles { get; } = roles;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous) return;

        var user = (User?)context.HttpContext.Items["User"];
        if (user == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (Roles.Count > 0 && !Roles.Contains(user.Role))
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
    }
}
