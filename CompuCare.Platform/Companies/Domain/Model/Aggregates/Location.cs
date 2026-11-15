using CompuCare.Platform.Companies.Domain.Model.Commands;
using CompuCare.Platform.Companies.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Domain.Model.Aggregates;

namespace CompuCare.Platform.Companies.Domain.Model.Aggregates;

/// <summary>
///     A site and desk of a company where equipment can be located.
/// </summary>
public class Location : AggregateRoot
{
    public Location()
    {
        Name = string.Empty;
        Desk = string.Empty;
    }

    public Location(int companyId, string name, string desk) : this()
    {
        if (companyId <= 0)
            throw new ArgumentException("A valid company is required.", nameof(companyId));
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(desk);
        CompanyId = companyId;
        Name = name.Trim();
        Desk = desk.Trim();
        Active = true;
    }

    public Location(CreateLocationCommand command) : this(command.CompanyId, command.Name, command.Desk)
    {
    }

    public LocationId Id { get; }
    public int CompanyId { get; private set; }
    public string Name { get; private set; }
    public string Desk { get; private set; }
    public bool Active { get; private set; }

    public Location Rename(string name, string desk)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(desk);
        Name = name.Trim();
        Desk = desk.Trim();
        return this;
    }

    public Location SetActive(bool active)
    {
        Active = active;
        return this;
    }
}
