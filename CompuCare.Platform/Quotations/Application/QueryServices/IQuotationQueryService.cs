using CompuCare.Platform.Quotations.Domain.Model.Aggregates;
using CompuCare.Platform.Quotations.Domain.Model.Queries;

namespace CompuCare.Platform.Quotations.Application.QueryServices;

/// <summary>
///     The quotation query service.
/// </summary>
public interface IQuotationQueryService
{
    Task<IEnumerable<Quotation>> Handle(GetAllQuotationsQuery query, CancellationToken cancellationToken);

    Task<IEnumerable<Quotation>> Handle(GetQuotationsByCompanyIdQuery query, CancellationToken cancellationToken);

    Task<IEnumerable<Quotation>> Handle(GetQuotationsByTechnicianIdQuery query, CancellationToken cancellationToken);
}
