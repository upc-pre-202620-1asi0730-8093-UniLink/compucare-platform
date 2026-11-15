using CompuCare.Platform.Iam.Domain.Model.Commands;
using CompuCare.Platform.Iam.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Iam.Interfaces.Rest.Transform;

public static class SignUpCompanyCommandFromResourceAssembler
{
    public static SignUpCompanyCommand ToCommandFromResource(SignUpResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new SignUpCompanyCommand(
            resource.Company?.Name ?? string.Empty,
            resource.Company?.Ruc ?? string.Empty,
            resource.Admin?.Name ?? string.Empty,
            resource.Admin?.Email ?? string.Empty,
            resource.Admin?.Password ?? string.Empty,
            resource.PlanId,
            resource.Payment?.Holder,
            resource.Payment?.Last4);
    }
}
