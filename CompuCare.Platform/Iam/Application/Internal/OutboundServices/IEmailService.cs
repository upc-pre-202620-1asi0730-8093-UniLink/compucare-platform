namespace CompuCare.Platform.Iam.Application.Internal.OutboundServices;

/// <summary>
///     Sends account emails (employee invitations and password recovery).
/// </summary>
public interface IEmailService
{
    /// <summary>
    ///     True when no email is actually sent (development). The caller then returns the secret
    ///     to the client so the flow can still be completed locally.
    /// </summary>
    bool IsSimulated { get; }

    Task SendAsync(string address, string subject, string body, CancellationToken cancellationToken);
}
