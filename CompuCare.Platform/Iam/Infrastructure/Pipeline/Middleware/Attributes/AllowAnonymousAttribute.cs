namespace CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
///     Marks a controller action as not requiring authorization.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute
{
}
