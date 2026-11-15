using System.Net;
using System.Net.Mail;
using CompuCare.Platform.Iam.Application.Internal.OutboundServices;
using CompuCare.Platform.Iam.Infrastructure.Email.Smtp.Configuration;
using Microsoft.Extensions.Options;

namespace CompuCare.Platform.Iam.Infrastructure.Email.Smtp.Services;

/// <summary>
///     Sends account emails through SMTP. In development no email is sent.
/// </summary>
public class EmailService(IOptions<SmtpSettings> smtpSettings, IHostEnvironment environment) : IEmailService
{
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

    public bool IsSimulated => environment.IsDevelopment();

    public async Task SendAsync(string address, string subject, string body, CancellationToken cancellationToken)
    {
        if (IsSimulated) return;
        if (string.IsNullOrWhiteSpace(_smtpSettings.Host) || string.IsNullOrWhiteSpace(_smtpSettings.From))
            throw new InvalidOperationException("Email delivery is not configured.");

        using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port) { EnableSsl = true };
        if (!string.IsNullOrWhiteSpace(_smtpSettings.User))
            smtpClient.Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Password);
        using var message = new MailMessage(_smtpSettings.From, address, subject, body);
        await smtpClient.SendMailAsync(message, cancellationToken);
    }
}
