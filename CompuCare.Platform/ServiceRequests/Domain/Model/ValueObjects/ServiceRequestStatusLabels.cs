namespace CompuCare.Platform.ServiceRequests.Domain.Model.ValueObjects;

/// <summary>
///     The status labels published to the web application. They are part of the HTTP contract,
///     so translating the user interface never changes them.
/// </summary>
public static class ServiceRequestStatusLabels
{
    public static string ToLabel(EServiceRequestStatus status) => status switch
    {
        EServiceRequestStatus.PendingAssignment => "Pendiente de Asignación",
        EServiceRequestStatus.Assigned => "Asignado",
        EServiceRequestStatus.InDiagnosis => "En Diagnóstico",
        EServiceRequestStatus.Resolved => "Atendido",
        EServiceRequestStatus.Cancelled => "Cancelado",
        _ => "Sin estado"
    };
}
