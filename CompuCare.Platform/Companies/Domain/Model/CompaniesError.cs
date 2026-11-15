namespace CompuCare.Platform.Companies.Domain.Model;

public enum CompaniesError
{
    None,
    InvalidCompanyData,
    LocationNotFound,
    InvalidLocationData,
    DuplicateLocation,
    EquipmentNotFound,
    InvalidEquipmentData,
    DuplicateSerialNumber,
    LocationUnavailable,
    EmployeeNotInCompany,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}
