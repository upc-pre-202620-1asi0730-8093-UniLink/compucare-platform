namespace CompuCare.Platform.Subscriptions.Interfaces.Rest.Resources;

public record PlanResource(int Id, string Code, string Name, decimal Price, decimal Hours, int PreventiveVisits);
