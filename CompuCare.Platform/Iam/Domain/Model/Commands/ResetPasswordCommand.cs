namespace CompuCare.Platform.Iam.Domain.Model.Commands;

/// <summary>
///     Sets a new password using a previously issued password reset token.
/// </summary>
public record ResetPasswordCommand(string Token, string NewPassword);
