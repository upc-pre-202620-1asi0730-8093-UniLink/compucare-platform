using CompuCare.Platform.Shared.Domain.Model.Errors;

namespace CompuCare.Platform.Companies.Domain.Model.Errors;

public static class CompaniesErrors
{
    public static readonly Error InvalidCompanyData =
        Error.Validation("Companies.InvalidCompanyData", "The company name or tax identifier is invalid.");

    public static readonly Error LocationNotFound =
        Error.NotFound("Companies.LocationNotFound", "The specified location was not found.");

    public static readonly Error InvalidLocationData =
        Error.Validation("Companies.InvalidLocationData", "The site and desk are required.");

    public static readonly Error DuplicateLocation =
        Error.Conflict("Companies.DuplicateLocation", "The company already has a location with this site and desk.");

    public static readonly Error EquipmentNotFound =
        Error.NotFound("Companies.EquipmentNotFound", "The specified equipment was not found.");

    public static readonly Error InvalidEquipmentData =
        Error.Validation("Companies.InvalidEquipmentData", "The name, serial number and type are required.");

    public static readonly Error DuplicateSerialNumber =
        Error.Conflict("Companies.DuplicateSerialNumber", "A device with this serial number already exists.");

    public static readonly Error LocationUnavailable =
        Error.Validation("Companies.LocationUnavailable",
            "The selected location does not belong to the company or is inactive.");

    public static readonly Error EmployeeNotInCompany =
        Error.Validation("Companies.EmployeeNotInCompany",
            "Equipment must be assigned to an active employee of its company.");
}
