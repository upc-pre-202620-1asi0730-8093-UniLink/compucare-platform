using CompuCare.Platform.Companies.Domain.Model.ValueObjects;

namespace CompuCare.Platform.Companies.Domain.Model.Queries;

/// <summary>
///     The get equipments by company id query, with optional filters.
/// </summary>
/// <param name="CompanyId">The company that owns the equipment.</param>
/// <param name="AssignedEmployeeId">When set, only equipment assigned to this employee.</param>
/// <param name="Status">When set, only equipment with this status.</param>
/// <param name="Type">When set, only equipment of this type.</param>
/// <param name="LocationId">When set, only equipment at this location.</param>
/// <param name="SearchTerm">When set, matched against code, name and serial number.</param>
public record GetEquipmentsByCompanyIdQuery(
    int CompanyId,
    int? AssignedEmployeeId = null,
    EEquipmentStatus? Status = null,
    string? Type = null,
    int? LocationId = null,
    string? SearchTerm = null);
