using CompuCare.Platform.Quotations.Domain.Model.Aggregates;
using CompuCare.Platform.Quotations.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Quotations.Interfaces.Rest.Transform;

public static class QuotationResourceFromEntityAssembler
{
    public static QuotationResource ToResourceFromEntity(Quotation quotation)
    {
        ArgumentNullException.ThrowIfNull(quotation);
        return new QuotationResource(quotation.Id, quotation.ServiceRequestId, quotation.CompanyId,
            quotation.TechnicianId,
            quotation.Items.Select(line => new QuotationLineResource(line.Description, line.Cost)).ToList(),
            quotation.Total, QuotationStatusLabelAssembler.ToLabelFromStatus(quotation.Status), quotation.CreatedAt,
            quotation.PaidAt, quotation.Receipt);
    }
}
