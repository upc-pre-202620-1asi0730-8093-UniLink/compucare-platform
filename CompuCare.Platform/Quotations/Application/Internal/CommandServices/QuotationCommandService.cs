using CompuCare.Platform.Quotations.Application.CommandServices;
using CompuCare.Platform.Quotations.Application.Internal.OutboundServices.Acl;
using CompuCare.Platform.Quotations.Domain.Model;
using CompuCare.Platform.Quotations.Domain.Model.Aggregates;
using CompuCare.Platform.Quotations.Domain.Model.Commands;
using CompuCare.Platform.Quotations.Domain.Model.Errors;
using CompuCare.Platform.Quotations.Domain.Model.ValueObjects;
using CompuCare.Platform.Quotations.Domain.Repositories;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Quotations.Application.Internal.CommandServices;

public class QuotationCommandService(
    IQuotationRepository quotationRepository,
    ExternalServiceRequestService externalServiceRequestService,
    ExternalUserService externalUserService,
    ExternalNotificationService externalNotificationService,
    IUnitOfWork unitOfWork)
    : IQuotationCommandService
{
    /// <inheritdoc />
    public async Task<Result<Quotation>> Handle(IssueQuotationCommand command, CancellationToken cancellationToken)
    {
        var serviceRequest = await externalServiceRequestService.FetchQuotableServiceRequest(command.ServiceRequestId,
            command.TechnicianId, cancellationToken);
        if (serviceRequest is null)
            return Result<Quotation>.Failure(QuotationsErrors.ServiceRequestNotFound,
                QuotationsError.ServiceRequestNotFound);
        if (!serviceRequest.AcceptsQuotations)
            return Result<Quotation>.Failure(QuotationsErrors.QuotationNotAllowed, QuotationsError.QuotationNotAllowed);

        Quotation quotation;
        try
        {
            quotation = new Quotation(serviceRequest.Id, serviceRequest.CompanyId, command.TechnicianId,
                (command.Items ?? []).Select(item => (item.Description, item.Cost)).ToList());
        }
        catch (ArgumentException)
        {
            return Result<Quotation>.Failure(QuotationsErrors.InvalidQuotationData,
                QuotationsError.InvalidQuotationData);
        }

        await quotationRepository.AddAsync(quotation, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        // The quotation number exists only after it is saved, so the managers are notified afterwards.
        var managers = await externalUserService.FetchActiveCompanyManagerIds(quotation.CompanyId, cancellationToken);
        await externalNotificationService.Notify(managers,
            $"A new quotation #{quotation.Id} requires your approval.", cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result<Quotation>.Success(quotation);
    }

    /// <inheritdoc />
    public async Task<Result<Quotation>> Handle(UpdateQuotationStatusCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Status is not (EQuotationStatus.Approved or EQuotationStatus.Rejected))
            return Result<Quotation>.Failure(QuotationsErrors.InvalidQuotationStatus,
                QuotationsError.InvalidQuotationStatus);

        var quotation = await quotationRepository.FindByIdAsync(command.QuotationId, cancellationToken);
        if (quotation is null || quotation.CompanyId != command.CompanyId)
            return Result<Quotation>.Failure(QuotationsErrors.QuotationNotFound, QuotationsError.QuotationNotFound);

        try
        {
            if (command.Status == EQuotationStatus.Approved) quotation.Approve();
            else quotation.Reject();
        }
        catch (InvalidOperationException)
        {
            return Result<Quotation>.Failure(QuotationsErrors.QuotationNotPending, QuotationsError.QuotationNotPending);
        }

        // A rejected quotation pauses the ticket until the technician issues an acceptable one.
        var siblings = await quotationRepository.FindByServiceRequestIdAsync(quotation.ServiceRequestId,
            cancellationToken);
        var hasRejected = quotation.Status == EQuotationStatus.Rejected ||
                          siblings.Any(q => q.Id != quotation.Id && q.Status == EQuotationStatus.Rejected);
        if (!await externalServiceRequestService.SetServiceRequestPaused(quotation.ServiceRequestId,
                command.CompanyId, hasRejected, cancellationToken))
            return Result<Quotation>.Failure(QuotationsErrors.ServiceRequestNotFound,
                QuotationsError.ServiceRequestNotFound);

        if (quotation.Status == EQuotationStatus.Approved)
            await externalNotificationService.Notify([quotation.TechnicianId],
                $"Quotation #{quotation.Id} approved.", cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Quotation>.Success(quotation);
    }

    /// <inheritdoc />
    public async Task<Result<Quotation>> Handle(PayQuotationCommand command, CancellationToken cancellationToken)
    {
        var quotation = await quotationRepository.FindByIdAsync(command.QuotationId, cancellationToken);
        if (quotation is null || quotation.CompanyId != command.CompanyId)
            return Result<Quotation>.Failure(QuotationsErrors.QuotationNotFound, QuotationsError.QuotationNotFound);

        if (string.IsNullOrWhiteSpace(command.Holder) || command.Last4 is not { Length: 4 } ||
            !command.Last4.All(char.IsDigit))
            return Result<Quotation>.Failure(QuotationsErrors.InvalidPaymentData, QuotationsError.InvalidPaymentData);

        try
        {
            quotation.RecordSimulatedPayment();
        }
        catch (InvalidOperationException)
        {
            return Result<Quotation>.Failure(QuotationsErrors.QuotationNotApproved,
                QuotationsError.QuotationNotApproved);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Quotation>.Success(quotation);
    }
}
