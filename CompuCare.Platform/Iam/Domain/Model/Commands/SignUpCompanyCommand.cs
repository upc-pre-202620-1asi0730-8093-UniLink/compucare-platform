namespace CompuCare.Platform.Iam.Domain.Model.Commands;

/// <summary>
///     Registers a company together with its first administrator and, optionally, a subscription plan.
/// </summary>
public record SignUpCompanyCommand(
    string CompanyName,
    string CompanyTaxId,
    string AdminName,
    string AdminEmail,
    string AdminPassword,
    int? PlanId,
    string? PaymentHolder,
    string? PaymentLast4);
