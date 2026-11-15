namespace CompuCare.Platform.ServiceRequests.Interfaces.Acl;

/// <summary>
///     Published language of the Service Requests context used to issue quotations.
/// </summary>
/// <param name="Id">The service request id.</param>
/// <param name="CompanyId">The company that reported the request.</param>
/// <param name="AcceptsQuotations">Whether the request is assigned or in diagnosis and not paused.</param>
public record QuotableServiceRequest(int Id, int CompanyId, bool AcceptsQuotations);
