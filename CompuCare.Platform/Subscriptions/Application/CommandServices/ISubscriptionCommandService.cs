using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Domain.Model.Commands;
using CompuCare.Platform.Shared.Application.Model;

namespace CompuCare.Platform.Subscriptions.Application.CommandServices;

/// <summary>
///     The subscription command service.
/// </summary>
public interface ISubscriptionCommandService
{
    Task<Result<Subscription>> Handle(ActivateSubscriptionCommand command, CancellationToken cancellationToken);

    Task<Result<Subscription>> Handle(SubscribeCommand command, CancellationToken cancellationToken);

    Task<Result<Subscription>> Handle(RenewSubscriptionCommand command, CancellationToken cancellationToken);
}
