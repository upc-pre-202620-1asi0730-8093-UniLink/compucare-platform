namespace CompuCare.Platform.Companies.Interfaces.Rest.Resources;

public record CreateEquipmentResource(string Name, string SerialNumber, string Type, int? LocationId);
