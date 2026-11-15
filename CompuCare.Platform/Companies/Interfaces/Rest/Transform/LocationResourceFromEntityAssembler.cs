using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Companies.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Companies.Interfaces.Rest.Transform;

public static class LocationResourceFromEntityAssembler
{
    public static LocationResource ToResourceFromEntity(Location location)
    {
        ArgumentNullException.ThrowIfNull(location);
        return new LocationResource(location.Id, location.CompanyId, location.Name, location.Desk, location.Active);
    }
}
