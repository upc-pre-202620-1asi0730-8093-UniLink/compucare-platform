namespace CompuCare.Platform.Iam.Infrastructure.Email.Smtp.Configuration;

/// <summary>
///     Binds the <c>Smtp</c> configuration section used to send account emails.
/// </summary>
public class SmtpSettings
{
    public string? Host { get; set; }
    public int Port { get; set; } = 587;
    public string? From { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
}
