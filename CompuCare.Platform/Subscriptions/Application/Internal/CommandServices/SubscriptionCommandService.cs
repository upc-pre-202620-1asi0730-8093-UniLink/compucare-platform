using CompuCare.Platform.Subscriptions.Application.CommandServices;
using CompuCare.Platform.Subscriptions.Domain.Model;
using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Domain.Model.Commands;
using CompuCare.Platform.Subscriptions.Domain.Model.Errors;
using CompuCare.Platform.Subscriptions.Domain.Repositories;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Subscriptions.Application.Internal.CommandServices;

public class SubscriptionCommandService(
    IPlanRepository planRepository,
    ISubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork)
    : ISubscriptionCommandService
{
    /// <inheritdoc />
    public async Task<Result<Subscription>> Handle(ActivateSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var plan = await planRepository.FindByIdAsync(command.PlanId, cancellationToken);
        if (plan is null)
            return Result<Subscription>.Failure(SubscriptionsErrors.PlanNotFound, SubscriptionsError.PlanNotFound);

        var subscription = await subscriptionRepository.FindByCompanyIdAsync(command.CompanyId, cancellationToken);
        if (subscription is null)
        {
            subscription = new Subscription(command.CompanyId, plan);
            await subscriptionRepository.AddAsync(subscription, cancellationToken);
        }
        else
        {
            subscription.Renew(plan);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Subscription>.Success(subscription);
    }

    /// <inheritdoc />
    public async Task<Result<Subscription>> Handle(SubscribeCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.BillingName) || string.IsNullOrWhiteSpace(command.BillingEmail) ||
            !command.BillingEmail.Contains('@'))
            return Result<Subscription>.Failure(SubscriptionsErrors.InvalidBillingData,
                SubscriptionsError.InvalidBillingData);

        return await Handle(new ActivateSubscriptionCommand(command.CompanyId, command.PlanId), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<Subscription>> Handle(RenewSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await subscriptionRepository.FindByCompanyIdAsync(command.CompanyId, cancellationToken);
        if (subscription is null)
            return Result<Subscription>.Failure(SubscriptionsErrors.SubscriptionNotFound,
                SubscriptionsError.SubscriptionNotFound);

        var plan = await planRepository.FindByIdAsync(subscription.PlanId, cancellationToken);
        if (plan is null)
            return Result<Subscription>.Failure(SubscriptionsErrors.PlanNotFound, SubscriptionsError.PlanNotFound);

        subscription.Renew(plan);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Subscription>.Success(subscription);
    }
}
