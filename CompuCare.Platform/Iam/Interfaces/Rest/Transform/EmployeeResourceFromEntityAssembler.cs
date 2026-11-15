using CompuCare.Platform.Iam.Domain.Model.Aggregates;
using CompuCare.Platform.Iam.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Iam.Interfaces.Rest.Transform;

public static class EmployeeResourceFromEntityAssembler
{
    public static EmployeeResource ToResourceFromEntity(User employee, string? temporaryPassword)
    {
        ArgumentNullException.ThrowIfNull(employee);
        return new EmployeeResource(employee.Id, employee.CompanyId, employee.Name, employee.Email, employee.Role,
            employee.Phone, employee.Position, temporaryPassword);
    }
}
