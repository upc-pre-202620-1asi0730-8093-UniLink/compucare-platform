namespace CompuCare.Platform.Subscriptions.Interfaces.Rest.Resources;

public record MaintenanceResource(int Id, int CompanyId, string Date, string Time, IEnumerable<int> EquipmentIds,
    string Status);
