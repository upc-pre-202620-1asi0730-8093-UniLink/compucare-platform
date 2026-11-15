namespace CompuCare.Platform.Quotations.Domain.Model.ValueObjects;

/// <summary>
///     The lifecycle of a quotation. The payment is academic: it only records a simulated receipt.
/// </summary>
public enum EQuotationStatus
{
    Pending,
    Approved,
    Rejected,
    PaidSimulated
}
