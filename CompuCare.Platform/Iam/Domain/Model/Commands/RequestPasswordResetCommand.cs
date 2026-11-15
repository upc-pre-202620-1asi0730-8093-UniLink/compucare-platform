namespace CompuCare.Platform.Iam.Domain.Model.Commands;

/// <summary>
///     Issues a one-time password reset token for the account registered with the given email.
/// </summary>
public record RequestPasswordResetCommand(string Email);
