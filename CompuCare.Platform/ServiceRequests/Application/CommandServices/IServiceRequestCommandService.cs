using CompuCare.Platform.ServiceRequests.Application.Model;
using CompuCare.Platform.ServiceRequests.Domain.Model.Commands;
using CompuCare.Platform.Shared.Application.Model;

namespace CompuCare.Platform.ServiceRequests.Application.CommandServices;

/// <summary>
///     The service request command service.
/// </summary>
public interface IServiceRequestCommandService
{
    Task<Result<ServiceRequestDetails>> Handle(ReportServiceRequestCommand command,
        CancellationToken cancellationToken);

    Task<Result<ServiceRequestDetails>> Handle(CancelServiceRequestCommand command,
        CancellationToken cancellationToken);

    Task<Result<ServiceRequestDetails>> Handle(AssignTechnicianCommand command, CancellationToken cancellationToken);

    Task<Result<ServiceRequestDetails>> Handle(StartDiagnosisCommand command, CancellationToken cancellationToken);

    Task<Result<ServiceRequestDetails>> Handle(ResolveServiceRequestCommand command,
        CancellationToken cancellationToken);
}
