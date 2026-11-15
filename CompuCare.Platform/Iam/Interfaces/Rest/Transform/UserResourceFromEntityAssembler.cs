using CompuCare.Platform.Iam.Domain.Model.Aggregates;
using CompuCare.Platform.Iam.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Iam.Interfaces.Rest.Transform;

public static class UserResourceFromEntityAssembler
{
    public static UserResource ToResourceFromEntity(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        return new UserResource(user.Id, user.CompanyId, user.Name, user.Email, user.Role, user.Phone,
            user.Position);
    }
}
