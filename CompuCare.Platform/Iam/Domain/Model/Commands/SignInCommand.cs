namespace CompuCare.Platform.Iam.Domain.Model.Commands;

/// <summary>
///     The sign-in command, carrying the email and password to authenticate.
/// </summary>
public record SignInCommand(string Email, string Password);
