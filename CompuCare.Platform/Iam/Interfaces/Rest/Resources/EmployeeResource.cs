namespace CompuCare.Platform.Iam.Interfaces.Rest.Resources;

public record EmployeeResource(int Id, int? CompanyId, string Name, string Email, string Role, string Phone,
    string Position, string? TempPassword);
