using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Companies.Domain.Model.ValueObjects;

namespace CompuCare.Platform.Companies.Application.Model;

/// <summary>
///     A piece of equipment together with the status derived from the Service Requests context.
/// </summary>
public record EquipmentWithStatus(Equipment Equipment, EEquipmentStatus Status);
