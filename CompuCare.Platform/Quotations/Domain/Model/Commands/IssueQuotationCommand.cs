namespace CompuCare.Platform.Quotations.Domain.Model.Commands;

/// <summary>
///     A priced item of a quotation being issued.
/// </summary>
public record IssueQuotationItem(string Description, decimal Cost);

/// <summary>
///     The assigned technician quotes additional parts or work for a service request.
/// </summary>
public record IssueQuotationCommand(int ServiceRequestId, int TechnicianId, IReadOnlyList<IssueQuotationItem> Items);
