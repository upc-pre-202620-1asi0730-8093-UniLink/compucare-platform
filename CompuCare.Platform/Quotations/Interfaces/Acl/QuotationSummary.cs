namespace CompuCare.Platform.Quotations.Interfaces.Acl;

/// <summary>
///     Published language of the Quotations context for an issued quotation.
/// </summary>
public record QuotationSummary(int Id, int ServiceRequestId, decimal Total, string Status);
