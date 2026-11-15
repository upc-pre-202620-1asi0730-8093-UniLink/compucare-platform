using CompuCare.Platform.Quotations.Domain.Model.Aggregates;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Quotations.Domain.Repositories;

/// <summary>
///     The quotation repository.
/// </summary>
public interface IQuotationRepository : IBaseRepository<Quotation>
{
    Task<IEnumerable<Quotation>> FindAllAsync(CancellationToken cancellationToken);

    Task<IEnumerable<Quotation>> FindByCompanyIdAsync(int companyId, CancellationToken cancellationToken);

    Task<IEnumerable<Quotation>> FindByTechnicianIdAsync(int technicianId, CancellationToken cancellationToken);

    Task<IEnumerable<Quotation>> FindByServiceRequestIdAsync(int serviceRequestId,
        CancellationToken cancellationToken);
}
