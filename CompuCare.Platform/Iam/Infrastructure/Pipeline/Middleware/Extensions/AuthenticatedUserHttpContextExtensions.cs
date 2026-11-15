using CompuCare.Platform.Iam.Interfaces.Acl;

namespace CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;

/// <summary>
///     Reads the <see cref="AuthenticatedUser" /> placed on the request by the authorization middleware.
/// </summary>
public static class AuthenticatedUserHttpContextExtensions
{
    /// <summary>
    ///     Returns the signed-in user. Only call it from actions protected by <c>[Authorize]</c>.
    /// </summary>
    public static AuthenticatedUser GetAuthenticatedUser(this HttpContext context)
    {
        return context.Items[AuthenticatedUser.HttpContextItemKey] as AuthenticatedUser
               ?? throw new InvalidOperationException("The request has no authenticated user.");
    }
}
