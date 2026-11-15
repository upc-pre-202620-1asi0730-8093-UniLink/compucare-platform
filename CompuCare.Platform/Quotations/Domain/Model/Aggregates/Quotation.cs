using System.Security.Cryptography;
using CompuCare.Platform.Quotations.Domain.Model.Entities;
using CompuCare.Platform.Quotations.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Domain.Model.Aggregates;

namespace CompuCare.Platform.Quotations.Domain.Model.Aggregates;

/// <summary>
///     The quotation aggregate root. Line item prices belong to this aggregate.
/// </summary>
public partial class Quotation : AggregateRoot
{
    private readonly List<QuotationLine> _items = [];

    public Quotation()
    {
    }

    public Quotation(int serviceRequestId, int companyId, int technicianId,
        IReadOnlyList<(string Description, decimal Cost)> items) : this()
    {
        if (serviceRequestId <= 0 || companyId <= 0 || technicianId <= 0)
            throw new ArgumentException("A service request, a company and a technician are required.");
        if (items is null || items.Count == 0 || items.Count > 50)
            throw new ArgumentException("A quotation must have between 1 and 50 items.", nameof(items));

        ServiceRequestId = serviceRequestId;
        CompanyId = companyId;
        TechnicianId = technicianId;
        foreach (var item in items) _items.Add(new QuotationLine(item.Description, item.Cost));
        Total = _items.Sum(line => line.Cost);
        Status = EQuotationStatus.Pending;
    }

    public QuotationId Id { get; }
    public int ServiceRequestId { get; private set; }
    public int CompanyId { get; private set; }
    public int TechnicianId { get; private set; }
    public decimal Total { get; private set; }
    public EQuotationStatus Status { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? Receipt { get; private set; }

    public IReadOnlyCollection<QuotationLine> Items => _items.AsReadOnly();

    /// <summary>
    ///     Whether the quotation still blocks the completion of its service request.
    /// </summary>
    public bool IsUnresolved => Status is EQuotationStatus.Pending or EQuotationStatus.Rejected;

    public void Approve()
    {
        if (Status != EQuotationStatus.Pending)
            throw new InvalidOperationException("Only a pending quotation can be approved.");
        Status = EQuotationStatus.Approved;
    }

    public void Reject()
    {
        if (Status != EQuotationStatus.Pending)
            throw new InvalidOperationException("Only a pending quotation can be rejected.");
        Status = EQuotationStatus.Rejected;
    }

    /// <summary>
    ///     Records an academic payment. No card data is kept and no payment gateway is called.
    /// </summary>
    public void RecordSimulatedPayment()
    {
        if (Status != EQuotationStatus.Approved)
            throw new InvalidOperationException("Only approved quotations accept simulated payment.");
        PaidAt = DateTime.UtcNow;
        Receipt = $"REC-{Convert.ToHexString(RandomNumberGenerator.GetBytes(5))}";
        Status = EQuotationStatus.PaidSimulated;
    }
}
