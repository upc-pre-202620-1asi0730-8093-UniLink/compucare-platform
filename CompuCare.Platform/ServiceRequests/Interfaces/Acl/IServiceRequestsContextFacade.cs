namespace CompuCare.Platform.ServiceRequests.Interfaces.Acl;

/// <summary>
///     Facade for the Service Requests context.
/// </summary>
public interface IServiceRequestsContextFacade
{
    /// <summary>
    ///     The ids of the company devices that have an open service request (they are in repair).
    /// </summary>
    Task<IEnumerable<int>> FetchEquipmentIdsWithOpenServiceRequests(int companyId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     The service requests reported for a device of the company, latest first.
    /// </summary>
    Task<IEnumerable<ServiceRequestSummary>> FetchServiceRequestSummariesByEquipmentId(int equipmentId, int companyId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     The service request assigned to the technician, or null when it does not exist or is not theirs.
    /// </summary>
    Task<QuotableServiceRequest?> FetchQuotableServiceRequest(int serviceRequestId, int technicianId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Pauses or resumes a service request of the company while a quotation is unresolved. The change is
    ///     tracked but not saved: the calling command commits it together with its own changes.
    /// </summary>
    /// <returns>False when the request does not exist, belongs to another company or is already closed.</returns>
    Task<bool> SetServiceRequestPaused(int serviceRequestId, int companyId, bool paused,
        CancellationToken cancellationToken);
}
