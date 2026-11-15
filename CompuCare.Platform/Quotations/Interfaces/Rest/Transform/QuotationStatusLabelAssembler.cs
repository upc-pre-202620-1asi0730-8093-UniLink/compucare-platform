using CompuCare.Platform.Quotations.Domain.Model.ValueObjects;

namespace CompuCare.Platform.Quotations.Interfaces.Rest.Transform;

/// <summary>
///     Translates quotation statuses to and from the codes used by the web application.
/// </summary>
public static class QuotationStatusLabelAssembler
{
    public static string ToLabelFromStatus(EQuotationStatus status) => status switch
    {
        EQuotationStatus.Pending => "PENDING",
        EQuotationStatus.Approved => "APPROVED",
        EQuotationStatus.Rejected => "REJECTED",
        EQuotationStatus.PaidSimulated => "PAID",
        _ => "UNKNOWN"
    };

    public static EQuotationStatus? ToStatusFromLabel(string? label) => label switch
    {
        "APPROVED" => EQuotationStatus.Approved,
        "REJECTED" => EQuotationStatus.Rejected,
        _ => null
    };
}
