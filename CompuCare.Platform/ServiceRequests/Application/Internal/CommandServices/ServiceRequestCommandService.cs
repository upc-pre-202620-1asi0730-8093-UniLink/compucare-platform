using CompuCare.Platform.ServiceRequests.Application.CommandServices;
using CompuCare.Platform.ServiceRequests.Application.Internal.OutboundServices.Acl;
using CompuCare.Platform.ServiceRequests.Application.Internal.QueryServices;
using CompuCare.Platform.ServiceRequests.Application.Model;
using CompuCare.Platform.ServiceRequests.Domain.Model;
using CompuCare.Platform.ServiceRequests.Domain.Model.Aggregates;
using CompuCare.Platform.ServiceRequests.Domain.Model.Commands;
using CompuCare.Platform.ServiceRequests.Domain.Model.Errors;
using CompuCare.Platform.ServiceRequests.Domain.Repositories;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Domain.Repositories;
using CompuCare.Platform.Subscriptions.Interfaces.Acl;
using DomainError = CompuCare.Platform.Shared.Domain.Model.Errors.Error;

namespace CompuCare.Platform.ServiceRequests.Application.Internal.CommandServices;

public class ServiceRequestCommandService(
    IServiceRequestRepository serviceRequestRepository,
    ExternalEquipmentService externalEquipmentService,
    ExternalUserService externalUserService,
    ExternalQuotationService externalQuotationService,
    ExternalSubscriptionService externalSubscriptionService,
    ExternalNotificationService externalNotificationService,
    ServiceRequestDetailsBuilder serviceRequestDetailsBuilder,
    IUnitOfWork unitOfWork)
    : IServiceRequestCommandService
{
    private const decimal CoverageAlertThreshold = 80m;

    /// <inheritdoc />
    public async Task<Result<ServiceRequestDetails>> Handle(ReportServiceRequestCommand command,
        CancellationToken cancellationToken)
    {
        if (!await externalEquipmentService.IsEquipmentAssignedToEmployee(command.EquipmentId, command.CompanyId,
                command.RequesterId, cancellationToken))
            return Failure(ServiceRequestsErrors.EquipmentNotAssignedToRequester,
                ServiceRequestsError.EquipmentNotAssignedToRequester);

        ServiceRequest serviceRequest;
        try
        {
            serviceRequest = new ServiceRequest(command);
        }
        catch (ArgumentException)
        {
            return Failure(ServiceRequestsErrors.InvalidServiceRequestData,
                ServiceRequestsError.InvalidServiceRequestData);
        }

        await serviceRequestRepository.AddAsync(serviceRequest, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return await Success(serviceRequest, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<ServiceRequestDetails>> Handle(CancelServiceRequestCommand command,
        CancellationToken cancellationToken)
    {
        var serviceRequest = await serviceRequestRepository.FindByIdAsync(command.ServiceRequestId, cancellationToken);
        if (serviceRequest is null || serviceRequest.RequesterId != command.RequesterId)
            return Failure(ServiceRequestsErrors.ServiceRequestNotFound, ServiceRequestsError.ServiceRequestNotFound);

        try
        {
            serviceRequest.Cancel(command.RequesterId);
        }
        catch (InvalidOperationException)
        {
            return Failure(ServiceRequestsErrors.InvalidStatusTransition,
                ServiceRequestsError.InvalidStatusTransition);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return await Success(serviceRequest, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<ServiceRequestDetails>> Handle(AssignTechnicianCommand command,
        CancellationToken cancellationToken)
    {
        var serviceRequest = await serviceRequestRepository.FindByIdAsync(command.ServiceRequestId, cancellationToken);
        if (serviceRequest is null)
            return Failure(ServiceRequestsErrors.ServiceRequestNotFound, ServiceRequestsError.ServiceRequestNotFound);

        if (!await externalUserService.ExistsActiveTechnician(command.TechnicianId, cancellationToken))
            return Failure(ServiceRequestsErrors.TechnicianNotFound, ServiceRequestsError.TechnicianNotFound);

        try
        {
            serviceRequest.AssignTo(command.TechnicianId);
        }
        catch (InvalidOperationException)
        {
            return Failure(ServiceRequestsErrors.InvalidStatusTransition,
                ServiceRequestsError.InvalidStatusTransition);
        }

        await externalNotificationService.Notify([command.TechnicianId],
            $"Ticket #{serviceRequest.Id} has been assigned to your account.", cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return await Success(serviceRequest, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<ServiceRequestDetails>> Handle(StartDiagnosisCommand command,
        CancellationToken cancellationToken)
    {
        var serviceRequest = await serviceRequestRepository.FindByIdAsync(command.ServiceRequestId, cancellationToken);
        if (serviceRequest is null || serviceRequest.TechnicianId != command.TechnicianId)
            return Failure(ServiceRequestsErrors.ServiceRequestNotFound, ServiceRequestsError.ServiceRequestNotFound);

        try
        {
            serviceRequest.StartDiagnosis(command.TechnicianId);
        }
        catch (InvalidOperationException)
        {
            return Failure(ServiceRequestsErrors.InvalidStatusTransition,
                ServiceRequestsError.InvalidStatusTransition);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return await Success(serviceRequest, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<ServiceRequestDetails>> Handle(ResolveServiceRequestCommand command,
        CancellationToken cancellationToken)
    {
        var serviceRequest = await serviceRequestRepository.FindByIdAsync(command.ServiceRequestId, cancellationToken);
        if (serviceRequest is null || serviceRequest.TechnicianId != command.TechnicianId)
            return Failure(ServiceRequestsErrors.ServiceRequestNotFound, ServiceRequestsError.ServiceRequestNotFound);

        var hasUnresolvedQuotations =
            await externalQuotationService.HasUnresolvedQuotations(serviceRequest.Id, cancellationToken);
        if (serviceRequest.Paused || hasUnresolvedQuotations)
            return Failure(ServiceRequestsErrors.UnresolvedQuotations, ServiceRequestsError.UnresolvedQuotations);

        try
        {
            serviceRequest.Resolve(command.TechnicianId, command.Hours, hasUnresolvedQuotations, command.Diagnosis,
                command.WorkDone);
        }
        catch (ArgumentException)
        {
            return Failure(ServiceRequestsErrors.InvalidResolutionData, ServiceRequestsError.InvalidResolutionData);
        }
        catch (InvalidOperationException)
        {
            return Failure(ServiceRequestsErrors.InvalidStatusTransition,
                ServiceRequestsError.InvalidStatusTransition);
        }

        var consumption = await externalSubscriptionService.RecordSupportHoursConsumption(serviceRequest.CompanyId,
            serviceRequest.Id, command.Hours, cancellationToken);
        switch (consumption.Outcome)
        {
            case ESupportHoursConsumptionOutcome.NoSubscription:
                return Failure(ServiceRequestsErrors.SubscriptionRequired, ServiceRequestsError.SubscriptionRequired);
            case ESupportHoursConsumptionOutcome.AlreadyRecorded:
                return Failure(ServiceRequestsErrors.SupportHoursAlreadyRegistered,
                    ServiceRequestsError.SupportHoursAlreadyRegistered);
            case ESupportHoursConsumptionOutcome.Expired:
                return Failure(ServiceRequestsErrors.SubscriptionExpired, ServiceRequestsError.SubscriptionExpired);
            case ESupportHoursConsumptionOutcome.InsufficientHours:
            case ESupportHoursConsumptionOutcome.Rejected:
                return Failure(ServiceRequestsErrors.InsufficientSupportHours,
                    ServiceRequestsError.InsufficientSupportHours);
        }

        if (consumption.PercentUsedBefore < CoverageAlertThreshold &&
            consumption.PercentUsedAfter >= CoverageAlertThreshold)
        {
            var managers = await externalUserService.FetchActiveCompanyManagerIds(serviceRequest.CompanyId,
                cancellationToken);
            await externalNotificationService.Notify(managers,
                "You have used 80% of your monthly support hours.", cancellationToken);
        }

        // One unit of work commits the request, the coverage consumption and the notifications together.
        await unitOfWork.CompleteAsync(cancellationToken);
        return await Success(serviceRequest, cancellationToken);
    }

    private async Task<Result<ServiceRequestDetails>> Success(ServiceRequest serviceRequest,
        CancellationToken cancellationToken)
    {
        var details = await serviceRequestDetailsBuilder.Build(serviceRequest, cancellationToken);
        return Result<ServiceRequestDetails>.Success(details);
    }

    private static Result<ServiceRequestDetails> Failure(DomainError error, ServiceRequestsError errorEnum) =>
        Result<ServiceRequestDetails>.Failure(error, errorEnum);
}
