using CompuCare.Platform.Shared.Domain.Model.Errors;

namespace CompuCare.Platform.Quotations.Domain.Model.Errors;

public static class QuotationsErrors
{
    public static readonly Error QuotationNotFound =
        Error.NotFound("Quotations.QuotationNotFound", "The specified quotation was not found.");

    public static readonly Error ServiceRequestNotFound =
        Error.NotFound("Quotations.ServiceRequestNotFound", "The specified service request was not found.");

    public static readonly Error QuotationNotAllowed =
        Error.Conflict("Quotations.QuotationNotAllowed", "Quotations are not permitted in the ticket's current state.");

    public static readonly Error InvalidQuotationData =
        Error.Validation("Quotations.InvalidQuotationData",
            "Between 1 and 50 items with a description and a positive cost are required.");

    public static readonly Error InvalidQuotationStatus =
        Error.Validation("Quotations.InvalidQuotationStatus", "Only APPROVED or REJECTED are allowed.");

    public static readonly Error QuotationNotPending =
        Error.Conflict("Quotations.QuotationNotPending", "Only a pending quotation can be approved or rejected.");

    public static readonly Error QuotationNotApproved =
        Error.Conflict("Quotations.QuotationNotApproved", "Only approved quotations accept simulated payment.");

    public static readonly Error InvalidPaymentData =
        Error.Validation("Quotations.InvalidPaymentData",
            "Only a fictitious cardholder and four test digits are allowed.");
}
