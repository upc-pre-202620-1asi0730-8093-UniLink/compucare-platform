using System.Security.Cryptography;
using CompuCare.Platform.Companies.Domain.Model.Commands;
using CompuCare.Platform.Companies.Domain.Model.Entities;
using CompuCare.Platform.Companies.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Domain.Model.Aggregates;

namespace CompuCare.Platform.Companies.Domain.Model.Aggregates;

/// <summary>
///     The equipment aggregate root. Users are referenced by id because they belong to the IAM context.
/// </summary>
public class Equipment : AggregateRoot
{
    private readonly List<EquipmentAssignment> _assignments = [];

    public Equipment()
    {
        Code = string.Empty;
        Name = string.Empty;
        SerialNumber = string.Empty;
        Type = string.Empty;
    }

    public Equipment(int companyId, string serialNumber, string name, string type, int? locationId = null) : this()
    {
        if (companyId <= 0)
            throw new ArgumentException("A valid company is required.", nameof(companyId));
        ArgumentException.ThrowIfNullOrWhiteSpace(serialNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        if (name.Trim().Length > 160 || serialNumber.Trim().Length > 100 || type.Trim().Length > 90)
            throw new ArgumentException("Equipment fields exceed the allowed length.");

        CompanyId = companyId;
        SerialNumber = serialNumber.Trim();
        Name = name.Trim();
        Type = type.Trim();
        LocationId = locationId;
        Code = $"EQ-{Convert.ToHexString(RandomNumberGenerator.GetBytes(4))}";
    }

    public Equipment(CreateEquipmentCommand command)
        : this(command.CompanyId, command.SerialNumber, command.Name, command.Type, command.LocationId)
    {
    }

    public EquipmentId Id { get; }
    public int CompanyId { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string SerialNumber { get; private set; }
    public string Type { get; private set; }
    public int? LocationId { get; private set; }
    public int? AssignedEmployeeId { get; private set; }

    public IReadOnlyCollection<EquipmentAssignment> Assignments => _assignments.AsReadOnly();

    /// <summary>
    ///     Assigns the equipment to an employee of the same company and records the change.
    /// </summary>
    public Equipment AssignTo(int employeeId, bool employeeBelongsToCompany)
    {
        if (employeeId <= 0 || !employeeBelongsToCompany)
            throw new InvalidOperationException("Equipment must be assigned to an employee of its company.");
        AssignedEmployeeId = employeeId;
        _assignments.Add(new EquipmentAssignment(employeeId));
        return this;
    }

    /// <summary>
    ///     Removes the current owner and records the change.
    /// </summary>
    public Equipment Unassign()
    {
        AssignedEmployeeId = null;
        _assignments.Add(new EquipmentAssignment(null));
        return this;
    }

    public bool IsAssignedTo(int employeeId) => AssignedEmployeeId == employeeId;
}
