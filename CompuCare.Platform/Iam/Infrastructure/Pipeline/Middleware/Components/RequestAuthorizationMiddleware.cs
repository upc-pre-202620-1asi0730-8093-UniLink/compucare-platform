using CompuCare.Platform.Iam.Application.Internal.OutboundServices;
using CompuCare.Platform.Iam.Application.QueryServices;
using CompuCare.Platform.Iam.Domain.Model.Queries;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using CompuCare.Platform.Iam.Interfaces.Acl;

namespace CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Components;

/// <summary>
///     Reads a bearer token off the request, validates it, and places the matching active user on
///     <c>HttpContext.Items["User"]</c> for <see cref="AuthorizeAttribute" /> to read later. The same user is
///     also published as an <see cref="AuthenticatedUser" /> for the other bounded contexts.
/// </summary>
public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IUserQueryService userQueryService,
        ITokenService tokenService)
    {
        var allowAnonymous = context.Request.HttpContext.GetEndpoint()?.Metadata
            .Any(m => m.GetType() == typeof(AllowAnonymousAttribute)) ?? false;

        if (allowAnonymous)
        {
            await next(context);
            return;
        }

        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            var userId = await tokenService.ValidateToken(token);

            if (userId != null)
            {
                var getUserByIdQuery = new GetUserByIdQuery(userId.Value);
                var user = await userQueryService.Handle(getUserByIdQuery, context.RequestAborted);
                if (user is { Active: true })
                {
                    context.Items["User"] = user;
                    context.Items[AuthenticatedUser.HttpContextItemKey] =
                        new AuthenticatedUser(user.Id.Value, user.Role, user.CompanyId);
                }
            }
        }

        await next(context);
    }
}
