using CompuCare.Platform.Quotations.Domain.Model.Aggregates;
using CompuCare.Platform.Quotations.Domain.Model.Commands;
using CompuCare.Platform.Shared.Application.Model;

namespace CompuCare.Platform.Quotations.Application.CommandServices;

/// <summary>
///     The quotation command service.
/// </summary>
public interface IQuotationCommandService
{
    Task<Result<Quotation>> Handle(IssueQuotationCommand command, CancellationToken cancellationToken);

    Task<Result<Quotation>> Handle(UpdateQuotationStatusCommand command, CancellationToken cancellationToken);

    Task<Result<Quotation>> Handle(PayQuotationCommand command, CancellationToken cancellationToken);
}
