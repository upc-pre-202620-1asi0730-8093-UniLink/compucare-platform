using CompuCare.Platform.ServiceRequests.Domain.Model.Commands;
using CompuCare.Platform.ServiceRequests.Domain.Model.Entities;
using CompuCare.Platform.ServiceRequests.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Domain.Model.Aggregates;

namespace CompuCare.Platform.ServiceRequests.Domain.Model.Aggregates;

/// <summary>
///     The service request aggregate root (core domain). The status only changes through its methods.
/// </summary>
public partial class ServiceRequest : AggregateRoot
{
    private readonly List<TimelineEntry> _timeline = [];

    public ServiceRequest()
    {
        Category = string.Empty;
        Description = string.Empty;
        Diagnosis = string.Empty;
        WorkDone = string.Empty;
    }

    public ServiceRequest(int companyId, int equipmentId, int requesterId, string description,
        string category = "Otro") : this()
    {
        if (companyId <= 0 || equipmentId <= 0 || requesterId <= 0)
            throw new ArgumentException("Valid company, equipment and requester identifiers are required.");
        if (string.IsNullOrWhiteSpace(description) || description.Trim().Length < 10)
            throw new ArgumentException("A description of at least ten characters is required.",
                nameof(description));
        if (string.IsNullOrWhiteSpace(category) || category.Trim().Length > 100)
            throw new ArgumentException("A request category is required.", nameof(category));

        CompanyId = companyId;
        EquipmentId = equipmentId;
        RequesterId = requesterId;
        Description = description.Trim();
        Category = category.Trim();
        Status = EServiceRequestStatus.PendingAssignment;
        Log(Status);
    }

    public ServiceRequest(ReportServiceRequestCommand command)
        : this(command.CompanyId, command.EquipmentId, command.RequesterId, command.Description, command.Category)
    {
    }

    public ServiceRequestId Id { get; }
    public int CompanyId { get; private set; }
    public int EquipmentId { get; private set; }
    public int RequesterId { get; private set; }
    public int? TechnicianId { get; private set; }
    public string Category { get; private set; }
    public string Description { get; private set; }
    public string Diagnosis { get; private set; }
    public string WorkDone { get; private set; }
    public EServiceRequestStatus Status { get; private set; }
    public bool Paused { get; private set; }
    public decimal WorkHours { get; private set; }

    public IReadOnlyCollection<TimelineEntry> Timeline => _timeline.AsReadOnly();

    /// <summary>
    ///     Whether the request is still being worked on, so its equipment is considered in repair.
    /// </summary>
    public bool IsOpen => Status is EServiceRequestStatus.PendingAssignment or EServiceRequestStatus.Assigned
        or EServiceRequestStatus.InDiagnosis;

    /// <summary>
    ///     Whether the assigned technician can still issue quotations for the request.
    /// </summary>
    public bool AcceptsQuotations => !Paused &&
                                     Status is EServiceRequestStatus.Assigned or EServiceRequestStatus.InDiagnosis;

    public void AssignTo(int technicianId)
    {
        if (Status != EServiceRequestStatus.PendingAssignment || technicianId <= 0)
            throw new InvalidOperationException("Only pending requests can be assigned to a valid technician.");
        TechnicianId = technicianId;
        Status = EServiceRequestStatus.Assigned;
        Log(Status);
    }

    public void StartDiagnosis(int technicianId)
    {
        if (Status != EServiceRequestStatus.Assigned || technicianId != TechnicianId)
            throw new InvalidOperationException("Only the assigned technician can start the diagnosis.");
        Status = EServiceRequestStatus.InDiagnosis;
        Log(Status);
    }

    public void Resolve(int technicianId, decimal hours, bool hasUnresolvedQuotations, string diagnosis,
        string workDone)
    {
        if (Status is not (EServiceRequestStatus.Assigned or EServiceRequestStatus.InDiagnosis) ||
            technicianId != TechnicianId)
            throw new InvalidOperationException(
                "Only the assigned technician can resolve an assigned or diagnosed request.");
        if (hours <= 0m)
            throw new ArgumentException("Work hours must be positive.", nameof(hours));
        if (string.IsNullOrWhiteSpace(diagnosis) || string.IsNullOrWhiteSpace(workDone))
            throw new ArgumentException("Diagnosis and completed work are required.");
        if (Paused || hasUnresolvedQuotations)
            throw new InvalidOperationException("Required quotations must be resolved before completing a ticket.");

        WorkHours = hours;
        Diagnosis = diagnosis.Trim();
        WorkDone = workDone.Trim();
        Status = EServiceRequestStatus.Resolved;
        Log(Status);
    }

    public void Cancel(int requesterId)
    {
        if (Status != EServiceRequestStatus.PendingAssignment || RequesterId != requesterId)
            throw new InvalidOperationException("Only the requester can cancel an unassigned request.");
        Status = EServiceRequestStatus.Cancelled;
        Log(Status);
    }

    public void SetPaused(bool paused)
    {
        if (Status is EServiceRequestStatus.Resolved or EServiceRequestStatus.Cancelled)
            throw new InvalidOperationException("Cannot pause a completed request.");
        Paused = paused;
    }

    private void Log(EServiceRequestStatus status) => _timeline.Add(new TimelineEntry(status));
}
