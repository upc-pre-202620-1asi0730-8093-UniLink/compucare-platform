using CompuCare.Platform.Iam.Domain.Model.Commands;
using CompuCare.Platform.Iam.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Iam.Interfaces.Rest.Transform;

public static class RequestPasswordResetCommandFromResourceAssembler
{
    public static RequestPasswordResetCommand ToCommandFromResource(RequestPasswordResetResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new RequestPasswordResetCommand(resource.Email);
    }
}
