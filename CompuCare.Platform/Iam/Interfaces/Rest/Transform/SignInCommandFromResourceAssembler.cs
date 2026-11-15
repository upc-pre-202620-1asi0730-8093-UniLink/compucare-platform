using CompuCare.Platform.Iam.Domain.Model.Commands;
using CompuCare.Platform.Iam.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Iam.Interfaces.Rest.Transform;

public static class SignInCommandFromResourceAssembler
{
    public static SignInCommand ToCommandFromResource(SignInResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new SignInCommand(resource.Email, resource.Password);
    }
}
