using CompuCare.Platform.Quotations.Domain.Repositories;
using CompuCare.Platform.Quotations.Interfaces.Acl;

namespace CompuCare.Platform.Quotations.Application.Acl;

/// <summary>
///     Facade for the Quotations context.
/// </summary>
public class QuotationsContextFacade(IQuotationRepository quotationRepository) : IQuotationsContextFacade
{
    /// <inheritdoc />
    public async Task<bool> HasUnresolvedQuotations(int serviceRequestId, CancellationToken cancellationToken)
    {
        var quotations = await quotationRepository.FindByServiceRequestIdAsync(serviceRequestId, cancellationToken);
        return quotations.Any(q => q.IsUnresolved);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<QuotationSummary>> FetchQuotationSummariesByServiceRequestId(int serviceRequestId,
        CancellationToken cancellationToken)
    {
        var quotations = await quotationRepository.FindByServiceRequestIdAsync(serviceRequestId, cancellationToken);
        return quotations.Select(q => new QuotationSummary(q.Id, q.ServiceRequestId, q.Total, q.Status.ToString()))
            .ToList();
    }
}
