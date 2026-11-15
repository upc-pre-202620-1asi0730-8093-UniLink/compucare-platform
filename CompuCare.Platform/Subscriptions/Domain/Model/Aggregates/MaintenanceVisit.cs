using CompuCare.Platform.Subscriptions.Domain.Model.Commands;
using CompuCare.Platform.Subscriptions.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Domain.Model.Aggregates;

namespace CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;

/// <summary>
///     A preventive maintenance visit booked against a subscription.
/// </summary>
public class MaintenanceVisit : AggregateRoot
{
    public MaintenanceVisit()
    {
        Date = string.Empty;
        Time = string.Empty;
        EquipmentIds = [];
        Status = "Agendado";
    }

    public MaintenanceVisit(ScheduleMaintenanceVisitCommand command, SubscriptionId subscriptionId) : this()
    {
        if (command.CompanyId <= 0)
            throw new ArgumentException("A valid company is required.", nameof(command));
        if (!DateOnly.TryParseExact(command.Date, "yyyy-MM-dd", out _))
            throw new ArgumentException("The date must use the yyyy-MM-dd format.", nameof(command));
        if (!TimeOnly.TryParse(command.Time, out _))
            throw new ArgumentException("A valid time is required.", nameof(command));
        if (command.EquipmentIds is null || command.EquipmentIds.Count == 0 ||
            command.EquipmentIds.Any(id => id <= 0) ||
            command.EquipmentIds.Distinct().Count() != command.EquipmentIds.Count)
            throw new ArgumentException("A non-duplicated equipment list is required.", nameof(command));

        CompanyId = command.CompanyId;
        SubscriptionId = subscriptionId;
        Date = command.Date;
        Time = command.Time;
        EquipmentIds = command.EquipmentIds.ToList();
    }

    public MaintenanceVisitId Id { get; }
    public int CompanyId { get; private set; }
    public SubscriptionId SubscriptionId { get; private set; }
    public string Date { get; private set; }
    public string Time { get; private set; }
    public List<int> EquipmentIds { get; private set; }
    public string Status { get; private set; }

    public bool Includes(int equipmentId) => EquipmentIds.Contains(equipmentId);
}
