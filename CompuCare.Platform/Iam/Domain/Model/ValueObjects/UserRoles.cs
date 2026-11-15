namespace CompuCare.Platform.Iam.Domain.Model.ValueObjects;

/// <summary>
///     The account roles recognized by CompuCare. The values are part of the HTTP contract.
/// </summary>
public static class UserRoles
{
    public const string SystemAdministrator = "sysadmin";
    public const string Technician = "technician";
    public const string Administrator = "admin";
    public const string CompanyManager = "company_manager";
    public const string Employee = "employee";

    private static readonly string[] KnownRoles =
        [SystemAdministrator, Technician, Administrator, CompanyManager, Employee];

    private static readonly string[] CompanyRoles = [Administrator, CompanyManager, Employee];

    public static bool IsKnown(string role) => KnownRoles.Contains(role);

    public static bool RequiresCompany(string role) => CompanyRoles.Contains(role);

    public static bool IsCompanyManager(string role) => role is Administrator or CompanyManager;
}
