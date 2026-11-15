using CompuCare.Platform.Quotations.Domain.Model.Commands;
using CompuCare.Platform.Quotations.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Quotations.Interfaces.Rest.Transform;

public static class PayQuotationCommandFromResourceAssembler
{
    public static PayQuotationCommand ToCommandFromResource(int quotationId, int companyId,
        PayQuotationResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new PayQuotationCommand(quotationId, companyId, resource.Holder ?? string.Empty,
            resource.Last4 ?? string.Empty);
    }
}
