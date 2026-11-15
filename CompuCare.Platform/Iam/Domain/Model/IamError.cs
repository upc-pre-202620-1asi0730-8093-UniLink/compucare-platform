namespace CompuCare.Platform.Iam.Domain.Model;

public enum IamError
{
    None,
    UserNotFound,
    EmailAlreadyRegistered,
    CompanyTaxIdAlreadyRegistered,
    InvalidCredentials,
    InvalidUserData,
    InvalidRegistrationData,
    InvalidPassword,
    CurrentPasswordMismatch,
    PlanNotFound,
    InvalidPaymentData,
    InvalidResetToken,
    UserUpdateForbidden,
    EmailDeliveryUnavailable,
    OperationCancelled,
    DatabaseError,
    InternalServerError,
    ExternalServiceError
}
