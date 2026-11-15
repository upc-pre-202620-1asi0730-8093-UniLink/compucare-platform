using CompuCare.Platform.Quotations.Domain.Model.ValueObjects;

namespace CompuCare.Platform.Quotations.Domain.Model.Commands;

/// <summary>
///     A company administrator approves or rejects a pending quotation.
/// </summary>
/// <param name="Status">Approved or Rejected; any other value is refused.</param>
public record UpdateQuotationStatusCommand(int QuotationId, int CompanyId, EQuotationStatus? Status);
