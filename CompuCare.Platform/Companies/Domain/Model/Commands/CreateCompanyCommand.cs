namespace CompuCare.Platform.Companies.Domain.Model.Commands;

/// <summary>
///     Registers a customer company.
/// </summary>
public record CreateCompanyCommand(string Name, string TaxId);
