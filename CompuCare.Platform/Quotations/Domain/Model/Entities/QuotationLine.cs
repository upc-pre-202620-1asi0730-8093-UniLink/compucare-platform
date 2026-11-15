namespace CompuCare.Platform.Quotations.Domain.Model.Entities;

/// <summary>
///     A priced line item of a quotation.
/// </summary>
public class QuotationLine
{
    public QuotationLine()
    {
        Description = string.Empty;
    }

    public QuotationLine(string description, decimal cost) : this()
    {
        if (string.IsNullOrWhiteSpace(description) || description.Trim().Length > 500)
            throw new ArgumentException("A description of up to 500 characters is required.", nameof(description));
        if (cost <= 0 || decimal.Round(cost, 2) != cost)
            throw new ArgumentException("The cost must be positive with up to two decimals.", nameof(cost));
        Description = description.Trim();
        Cost = cost;
    }

    public int Id { get; }
    public string Description { get; private set; }
    public decimal Cost { get; private set; }
}
