namespace CompuCare.Platform.Iam.Interfaces.Acl;

/// <summary>
///     Published language of the IAM context describing the signed-in user of the current request.
///     Other bounded contexts read it instead of the internal <c>User</c> aggregate.
/// </summary>
/// <param name="UserId">The id of the signed-in user.</param>
/// <param name="Role">The role of the signed-in user.</param>
/// <param name="CompanyId">The company of the signed-in user, if any.</param>
public record AuthenticatedUser(int UserId, string Role, int? CompanyId)
{
    public const string HttpContextItemKey = "AuthenticatedUser";
}
