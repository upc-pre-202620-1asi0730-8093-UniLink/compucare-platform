using CompuCare.Platform.Iam.Domain.Model.Commands;
using CompuCare.Platform.Iam.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Iam.Interfaces.Rest.Transform;

public static class ChangeUserPasswordCommandFromResourceAssembler
{
    public static ChangeUserPasswordCommand ToCommandFromResource(int userId, int requesterId,
        ChangeUserPasswordResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new ChangeUserPasswordCommand(userId, requesterId, resource.CurrentPassword, resource.NewPassword);
    }
}
