using CompuCare.Platform.Iam.Domain.Model.Commands;
using CompuCare.Platform.Iam.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Iam.Interfaces.Rest.Transform;

public static class CreateEmployeeCommandFromResourceAssembler
{
    public static CreateEmployeeCommand ToCommandFromResource(CreateEmployeeResource resource, int companyId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateEmployeeCommand(companyId, resource.Name, resource.Email, resource.Position);
    }
}
