namespace CompuCare.Platform.Subscriptions.Interfaces.Rest.Resources;

public record ScheduleMaintenanceResource(string Date, string Time, int[] EquipmentIds);
