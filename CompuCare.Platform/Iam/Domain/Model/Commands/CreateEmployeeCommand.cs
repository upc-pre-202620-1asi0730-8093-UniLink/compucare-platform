namespace CompuCare.Platform.Iam.Domain.Model.Commands;

/// <summary>
///     Creates an employee account inside the company of the requesting administrator.
/// </summary>
public record CreateEmployeeCommand(int CompanyId, string Name, string Email, string? Position);
