using CompuCare.Platform.Iam.Domain.Model.Commands;
using CompuCare.Platform.Shared.Application.Model;

namespace CompuCare.Platform.Iam.Application.CommandServices;

/// <summary>
///     The password reset command service.
/// </summary>
public interface IPasswordResetCommandService
{
    /// <returns>The plain reset token when email delivery is simulated (development), otherwise null.</returns>
    Task<Result<string?>> Handle(RequestPasswordResetCommand command, CancellationToken cancellationToken);

    Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken);
}
