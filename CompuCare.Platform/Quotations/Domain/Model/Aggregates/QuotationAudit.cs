using CompuCare.Platform.Shared.Domain.Model.Entities;

namespace CompuCare.Platform.Quotations.Domain.Model.Aggregates;

public partial class Quotation : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
