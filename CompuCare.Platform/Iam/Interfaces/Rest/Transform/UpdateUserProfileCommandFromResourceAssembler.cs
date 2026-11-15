using CompuCare.Platform.Iam.Domain.Model.Commands;
using CompuCare.Platform.Iam.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Iam.Interfaces.Rest.Transform;

public static class UpdateUserProfileCommandFromResourceAssembler
{
    public static UpdateUserProfileCommand ToCommandFromResource(int userId, int requesterId,
        UpdateUserProfileResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateUserProfileCommand(userId, requesterId, resource.Name, resource.Phone);
    }
}
