namespace CompuCare.Platform.Iam.Domain.Model.Commands;

/// <summary>
///     Changes the password of a user after verifying the current one.
/// </summary>
public record ChangeUserPasswordCommand(int UserId, int RequesterId, string CurrentPassword, string NewPassword);
