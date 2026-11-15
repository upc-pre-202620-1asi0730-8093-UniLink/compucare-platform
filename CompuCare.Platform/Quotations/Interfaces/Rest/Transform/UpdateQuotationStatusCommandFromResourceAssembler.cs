using CompuCare.Platform.Quotations.Domain.Model.Commands;
using CompuCare.Platform.Quotations.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Quotations.Interfaces.Rest.Transform;

public static class UpdateQuotationStatusCommandFromResourceAssembler
{
    public static UpdateQuotationStatusCommand ToCommandFromResource(int quotationId, int companyId,
        UpdateQuotationStatusResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateQuotationStatusCommand(quotationId, companyId,
            QuotationStatusLabelAssembler.ToStatusFromLabel(resource.Status));
    }
}
