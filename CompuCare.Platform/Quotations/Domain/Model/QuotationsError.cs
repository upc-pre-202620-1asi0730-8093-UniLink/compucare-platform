namespace CompuCare.Platform.Quotations.Domain.Model;

public enum QuotationsError
{
    None,
    QuotationNotFound,
    ServiceRequestNotFound,
    QuotationNotAllowed,
    InvalidQuotationData,
    InvalidQuotationStatus,
    QuotationNotPending,
    QuotationNotApproved,
    InvalidPaymentData,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}
