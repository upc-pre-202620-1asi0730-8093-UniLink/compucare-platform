using CompuCare.Platform.Iam.Domain.Model.Aggregates;
using CompuCare.Platform.Iam.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Iam.Interfaces.Rest.Transform;

public static class RegisteredCompanyResourceFromEntityAssembler
{
    public static RegisteredCompanyResource ToResourceFromEntity(User admin, string companyName, string companyTaxId)
    {
        ArgumentNullException.ThrowIfNull(admin);
        return new RegisteredCompanyResource(UserResourceFromEntityAssembler.ToResourceFromEntity(admin),
            new SignUpCompanyDataResource(companyName, companyTaxId));
    }
}
