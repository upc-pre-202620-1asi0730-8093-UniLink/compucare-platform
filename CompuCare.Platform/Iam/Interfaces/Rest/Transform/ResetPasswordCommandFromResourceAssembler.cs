using CompuCare.Platform.Iam.Domain.Model.Commands;
using CompuCare.Platform.Iam.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Iam.Interfaces.Rest.Transform;

public static class ResetPasswordCommandFromResourceAssembler
{
    public static ResetPasswordCommand ToCommandFromResource(ResetPasswordResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new ResetPasswordCommand(resource.Token, resource.Password);
    }
}
