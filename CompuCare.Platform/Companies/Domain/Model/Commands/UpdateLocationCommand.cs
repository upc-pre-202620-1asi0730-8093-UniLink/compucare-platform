namespace CompuCare.Platform.Companies.Domain.Model.Commands;

/// <summary>
///     Renames and/or activates or deactivates a company location. Null values are left unchanged.
/// </summary>
public record UpdateLocationCommand(int LocationId, int CompanyId, string? Name, string? Desk, bool? Active);
