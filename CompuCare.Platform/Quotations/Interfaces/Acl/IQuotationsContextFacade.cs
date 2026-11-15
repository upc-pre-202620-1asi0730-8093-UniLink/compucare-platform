namespace CompuCare.Platform.Quotations.Interfaces.Acl;

/// <summary>
///     Facade for the Quotations context.
/// </summary>
public interface IQuotationsContextFacade
{
    /// <summary>
    ///     Whether the service request has a pending or rejected quotation, which blocks its completion.
    /// </summary>
    Task<bool> HasUnresolvedQuotations(int serviceRequestId, CancellationToken cancellationToken);

    /// <summary>
    ///     The quotations issued for a service request.
    /// </summary>
    Task<IEnumerable<QuotationSummary>> FetchQuotationSummariesByServiceRequestId(int serviceRequestId,
        CancellationToken cancellationToken);
}
