namespace CompuCare.Platform.Quotations.Domain.Model.Commands;

/// <summary>
///     A company administrator records the simulated payment of an approved quotation.
/// </summary>
public record PayQuotationCommand(int QuotationId, int CompanyId, string Holder, string Last4);
