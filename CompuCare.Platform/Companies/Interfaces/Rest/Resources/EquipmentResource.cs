namespace CompuCare.Platform.Companies.Interfaces.Rest.Resources;

public record EquipmentResource(
    int Id,
    int CompanyId,
    string Code,
    string Name,
    string SerialNumber,
    string Type,
    int? LocationId,
    int? AssignedTo,
    string Status);
