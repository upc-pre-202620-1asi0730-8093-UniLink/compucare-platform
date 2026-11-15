namespace CompuCare.Platform.Quotations.Interfaces.Rest.Resources;

public record IssueQuotationResource(int TicketId, List<QuotationLineResource> Items);
