namespace CompuCare.Platform.Companies.Domain.Model.Commands;

/// <summary>
///     Registers a piece of equipment for a company.
/// </summary>
public record CreateEquipmentCommand(int CompanyId, string Name, string SerialNumber, string Type, int? LocationId);
