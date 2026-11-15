namespace CompuCare.Platform.Iam.Interfaces.Rest.Resources;

public record SignUpCompanyDataResource(string Name, string Ruc);

public record SignUpAdminResource(string Name, string Email, string Password);

public record SignUpPaymentResource(string Holder, string Last4);

public record SignUpResource(
    SignUpCompanyDataResource Company,
    SignUpAdminResource Admin,
    int? PlanId,
    SignUpPaymentResource? Payment);
