using System.Security.Cryptography;
using System.Text;
using CompuCare.Platform.Iam.Application.CommandServices;
using CompuCare.Platform.Iam.Application.Internal.OutboundServices;
using CompuCare.Platform.Iam.Domain.Model;
using CompuCare.Platform.Iam.Domain.Model.Aggregates;
using CompuCare.Platform.Iam.Domain.Model.Commands;
using CompuCare.Platform.Iam.Domain.Model.Errors;
using CompuCare.Platform.Iam.Domain.Repositories;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Iam.Application.Internal.CommandServices;

public class PasswordResetCommandService(
    IUserRepository userRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IHashingService hashingService,
    IEmailService emailService,
    IUnitOfWork unitOfWork)
    : IPasswordResetCommandService
{
    private const int MinimumPasswordLength = 6;

    private static string Digest(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    /// <inheritdoc />
    public async Task<Result<string?>> Handle(RequestPasswordResetCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
            return Result<string?>.Success(null);

        var user = await userRepository.FindByEmailAsync(command.Email.Trim().ToLowerInvariant(), cancellationToken);
        // Never reveal whether an account exists for the given email.
        if (user is null || !user.Active)
            return Result<string?>.Success(null);

        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
        var resetToken = new PasswordResetToken(user.Id, Digest(token), DateTime.UtcNow.AddHours(1));

        try
        {
            await emailService.SendAsync(user.Email, "CompuCare password recovery",
                $"Your one-hour password reset token is: {token}", cancellationToken);
        }
        catch (InvalidOperationException)
        {
            return Result<string?>.Failure(IamErrors.EmailDeliveryUnavailable, IamError.EmailDeliveryUnavailable);
        }

        await passwordResetTokenRepository.AddAsync(resetToken, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<string?>.Success(emailService.IsSimulated ? token : null);
    }

    /// <inheritdoc />
    public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Token))
            return Result.Failure(IamErrors.InvalidResetToken, IamError.InvalidResetToken);

        var resetToken = await passwordResetTokenRepository.FindByTokenHashAsync(Digest(command.Token.Trim()),
            cancellationToken);
        if (resetToken is null || !resetToken.IsUsable)
            return Result.Failure(IamErrors.InvalidResetToken, IamError.InvalidResetToken);

        if (command.NewPassword is null || command.NewPassword.Length < MinimumPasswordLength)
            return Result.Failure(IamErrors.InvalidPassword, IamError.InvalidPassword);

        var user = await userRepository.FindByIdAsync(resetToken.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(IamErrors.InvalidResetToken, IamError.InvalidResetToken);

        resetToken.Consume();
        user.UpdatePasswordHash(hashingService.HashPassword(command.NewPassword));
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
