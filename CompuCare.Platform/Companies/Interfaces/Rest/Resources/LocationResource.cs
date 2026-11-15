namespace CompuCare.Platform.Companies.Interfaces.Rest.Resources;

public record LocationResource(int Id, int CompanyId, string Name, string Desk, bool Active);
