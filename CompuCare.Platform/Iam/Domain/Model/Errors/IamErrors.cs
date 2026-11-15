using CompuCare.Platform.Shared.Domain.Model.Errors;

namespace CompuCare.Platform.Iam.Domain.Model.Errors;

public static class IamErrors
{
    public static readonly Error InvalidCredentials =
        Error.Unauthorized("Iam.InvalidCredentials", "Invalid email or password.");

    public static readonly Error EmailAlreadyRegistered =
        Error.Conflict("Iam.EmailAlreadyRegistered", "The email is already registered.");

    public static readonly Error CompanyTaxIdAlreadyRegistered =
        Error.Conflict("Iam.CompanyTaxIdAlreadyRegistered", "The company tax identifier is already registered.");

    public static readonly Error UserNotFound =
        Error.NotFound("Iam.UserNotFound", "The specified user was not found.");

    public static readonly Error InvalidUserData =
        Error.Validation("Iam.InvalidUserData", "The name or email provided is invalid.");

    public static readonly Error InvalidRegistrationData =
        Error.Validation("Iam.InvalidRegistrationData",
            "Company, RUC (11 digits) and administrator information are required.");

    public static readonly Error InvalidPassword =
        Error.Validation("Iam.InvalidPassword", "The password must contain at least six characters.");

    public static readonly Error CurrentPasswordMismatch =
        Error.Validation("Iam.CurrentPasswordMismatch", "The current password does not match.");

    public static readonly Error PlanNotFound =
        Error.NotFound("Iam.PlanNotFound", "The selected plan was not found.");

    public static readonly Error InvalidPaymentData =
        Error.Validation("Iam.InvalidPaymentData", "A fictitious cardholder and four test digits are required.");

    public static readonly Error InvalidResetToken =
        Error.Validation("Iam.InvalidResetToken", "The reset token is invalid or expired.");

    public static readonly Error UserUpdateForbidden =
        Error.Forbidden("Iam.UserUpdateForbidden", "You cannot modify another user's account.");

    public static readonly Error EmailDeliveryUnavailable =
        Error.Failure("Iam.EmailDeliveryUnavailable", "Email delivery is not configured.");

    public static readonly Error ExternalServiceError =
        Error.Failure("Iam.ExternalServiceError", "A bounded context required to complete this request failed.");
}
