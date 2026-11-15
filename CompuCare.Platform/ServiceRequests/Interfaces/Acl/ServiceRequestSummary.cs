namespace CompuCare.Platform.ServiceRequests.Interfaces.Acl;

/// <summary>
///     Published language of the Service Requests context for a reported issue.
/// </summary>
public record ServiceRequestSummary(int Id, string Category, string Description, string Status);
