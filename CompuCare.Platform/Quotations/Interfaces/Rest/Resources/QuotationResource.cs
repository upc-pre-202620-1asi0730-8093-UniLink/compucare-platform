namespace CompuCare.Platform.Quotations.Interfaces.Rest.Resources;

public record QuotationLineResource(string Description, decimal Cost);

public record QuotationResource(
    int Id,
    int TicketId,
    int CompanyId,
    int TechnicianId,
    IEnumerable<QuotationLineResource> Items,
    decimal Total,
    string Status,
    DateTimeOffset? CreatedAt,
    DateTime? PaidAt,
    string? Receipt);
