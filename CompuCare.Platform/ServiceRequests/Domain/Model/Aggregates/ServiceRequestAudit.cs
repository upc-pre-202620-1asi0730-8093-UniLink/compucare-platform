using CompuCare.Platform.Shared.Domain.Model.Entities;

namespace CompuCare.Platform.ServiceRequests.Domain.Model.Aggregates;

public partial class ServiceRequest : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
