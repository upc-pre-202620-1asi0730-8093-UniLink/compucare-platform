using CompuCare.Platform.Quotations.Application.QueryServices;
using CompuCare.Platform.Quotations.Domain.Model.Aggregates;
using CompuCare.Platform.Quotations.Domain.Model.Queries;
using CompuCare.Platform.Quotations.Domain.Repositories;

namespace CompuCare.Platform.Quotations.Application.Internal.QueryServices;

public class QuotationQueryService(IQuotationRepository quotationRepository) : IQuotationQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Quotation>> Handle(GetAllQuotationsQuery query, CancellationToken cancellationToken)
    {
        return await quotationRepository.FindAllAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Quotation>> Handle(GetQuotationsByCompanyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await quotationRepository.FindByCompanyIdAsync(query.CompanyId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Quotation>> Handle(GetQuotationsByTechnicianIdQuery query,
        CancellationToken cancellationToken)
    {
        return await quotationRepository.FindByTechnicianIdAsync(query.TechnicianId, cancellationToken);
    }
}
