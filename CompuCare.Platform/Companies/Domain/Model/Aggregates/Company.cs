using CompuCare.Platform.Companies.Domain.Model.Commands;
using CompuCare.Platform.Companies.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Domain.Model.Aggregates;

namespace CompuCare.Platform.Companies.Domain.Model.Aggregates;

/// <summary>
///     The company aggregate root: a customer organization identified by its tax id (RUC).
/// </summary>
public class Company : AggregateRoot
{
    public Company()
    {
        Name = string.Empty;
        TaxId = string.Empty;
    }

    public Company(CreateCompanyCommand command) : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.TaxId);
        Name = command.Name.Trim();
        TaxId = command.TaxId.Trim();
    }

    public CompanyId Id { get; }
    public string Name { get; private set; }
    public string TaxId { get; private set; }
}
