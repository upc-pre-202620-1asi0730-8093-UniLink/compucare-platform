using CompuCare.Platform.Quotations.Domain.Model.Commands;
using CompuCare.Platform.Quotations.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Quotations.Interfaces.Rest.Transform;

public static class IssueQuotationCommandFromResourceAssembler
{
    public static IssueQuotationCommand ToCommandFromResource(IssueQuotationResource resource, int technicianId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new IssueQuotationCommand(resource.TicketId, technicianId,
            (resource.Items ?? []).Select(item => new IssueQuotationItem(item.Description, item.Cost)).ToList());
    }
}
