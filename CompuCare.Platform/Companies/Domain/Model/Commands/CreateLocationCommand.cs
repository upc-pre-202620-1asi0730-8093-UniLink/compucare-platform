namespace CompuCare.Platform.Companies.Domain.Model.Commands;

/// <summary>
///     Registers a site and desk for a company.
/// </summary>
public record CreateLocationCommand(int CompanyId, string Name, string Desk);
