using CompuCare.Platform.ServiceRequests.Domain.Model.ValueObjects;

namespace CompuCare.Platform.ServiceRequests.Domain.Model.Entities;

/// <summary>
///     One step of the timeline of a service request.
/// </summary>
public class TimelineEntry
{
    public TimelineEntry()
    {
    }

    public TimelineEntry(EServiceRequestStatus status)
    {
        Status = status;
        At = DateTime.UtcNow;
    }

    public int Id { get; }
    public EServiceRequestStatus Status { get; private set; }
    public DateTime At { get; private set; }
}
