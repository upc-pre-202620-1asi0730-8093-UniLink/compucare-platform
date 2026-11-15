using System.Security.Cryptography;
using CompuCare.Platform.Iam.Application.CommandServices;
using CompuCare.Platform.Iam.Application.Internal.OutboundServices;
using CompuCare.Platform.Iam.Application.Internal.OutboundServices.Acl;
using CompuCare.Platform.Iam.Domain.Model;
using CompuCare.Platform.Iam.Domain.Model.Aggregates;
using CompuCare.Platform.Iam.Domain.Model.Commands;
using CompuCare.Platform.Iam.Domain.Model.Errors;
using CompuCare.Platform.Iam.Domain.Model.ValueObjects;
using CompuCare.Platform.Iam.Domain.Repositories;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Iam.Application.Internal.CommandServices;

public class UserCommandService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    IEmailService emailService,
    ExternalCompanyService externalCompanyService,
    ExternalSubscriptionService externalSubscriptionService,
    IUnitOfWork unitOfWork)
    : IUserCommandService
{
    private const int MinimumPasswordLength = 6;

    /// <inheritdoc />
    public async Task<Result<(User user, string token)>> Handle(SignInCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrWhiteSpace(command.Password))
            return Result<(User user, string token)>.Failure(IamErrors.InvalidCredentials, IamError.InvalidCredentials);

        var email = command.Email.Trim().ToLowerInvariant();
        var user = await userRepository.FindByEmailAsync(email, cancellationToken);

        if (user is null || !user.Active || !hashingService.VerifyPassword(command.Password, user.PasswordHash))
            return Result<(User user, string token)>.Failure(IamErrors.InvalidCredentials, IamError.InvalidCredentials);

        var token = tokenService.GenerateToken(user);
        return Result<(User user, string token)>.Success((user, token));
    }

    /// <inheritdoc />
    public async Task<Result<(User admin, string companyName, string companyTaxId)>> Handle(
        SignUpCompanyCommand command, CancellationToken cancellationToken)
    {
        string adminEmail;
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(command.CompanyName);
            ArgumentException.ThrowIfNullOrWhiteSpace(command.AdminName);
            ArgumentException.ThrowIfNullOrWhiteSpace(command.AdminPassword);
            if (command.CompanyTaxId is not { Length: 11 } || !command.CompanyTaxId.All(char.IsDigit))
                throw new ArgumentException("The RUC must contain eleven digits.", nameof(command.CompanyTaxId));
            adminEmail = User.NormalizeEmail(command.AdminEmail);
        }
        catch (ArgumentException)
        {
            return Failure(IamErrors.InvalidRegistrationData, IamError.InvalidRegistrationData);
        }

        if (command.AdminPassword.Length < MinimumPasswordLength)
            return Failure(IamErrors.InvalidPassword, IamError.InvalidPassword);

        if (await externalCompanyService.ExistsCompanyByTaxId(command.CompanyTaxId, cancellationToken))
            return Failure(IamErrors.CompanyTaxIdAlreadyRegistered, IamError.CompanyTaxIdAlreadyRegistered);

        if (await userRepository.ExistsByEmailAsync(adminEmail, cancellationToken))
            return Failure(IamErrors.EmailAlreadyRegistered, IamError.EmailAlreadyRegistered);

        if (command.PlanId.HasValue)
        {
            if (!await externalSubscriptionService.ExistsPlanById(command.PlanId.Value, cancellationToken))
                return Failure(IamErrors.PlanNotFound, IamError.PlanNotFound);
            if (string.IsNullOrWhiteSpace(command.PaymentHolder) ||
                command.PaymentLast4 is not { Length: 4 } || !command.PaymentLast4.All(char.IsDigit))
                return Failure(IamErrors.InvalidPaymentData, IamError.InvalidPaymentData);
        }

        var companyId = await externalCompanyService.CreateCompany(command.CompanyName.Trim(),
            command.CompanyTaxId, cancellationToken);
        if (companyId is null)
            return Failure(IamErrors.ExternalServiceError, IamError.ExternalServiceError);

        var admin = new User(command.AdminName, adminEmail, hashingService.HashPassword(command.AdminPassword),
            UserRoles.Administrator, companyId, "Administrador");
        await userRepository.AddAsync(admin, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        if (command.PlanId.HasValue &&
            await externalSubscriptionService.ActivateSubscription(companyId.Value, command.PlanId.Value,
                cancellationToken) is null)
            return Failure(IamErrors.ExternalServiceError, IamError.ExternalServiceError);

        return Result<(User admin, string companyName, string companyTaxId)>.Success(
            (admin, command.CompanyName.Trim(), command.CompanyTaxId));

        static Result<(User admin, string companyName, string companyTaxId)> Failure(
            Shared.Domain.Model.Errors.Error error, IamError errorEnum) =>
            Result<(User admin, string companyName, string companyTaxId)>.Failure(error, errorEnum);
    }

    /// <inheritdoc />
    public async Task<Result<(User employee, string? temporaryPassword)>> Handle(CreateEmployeeCommand command,
        CancellationToken cancellationToken)
    {
        string email;
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(command.Name);
            email = User.NormalizeEmail(command.Email);
        }
        catch (ArgumentException)
        {
            return Result<(User employee, string? temporaryPassword)>.Failure(IamErrors.InvalidUserData,
                IamError.InvalidUserData);
        }

        if (await userRepository.ExistsByEmailAsync(email, cancellationToken))
            return Result<(User employee, string? temporaryPassword)>.Failure(IamErrors.EmailAlreadyRegistered,
                IamError.EmailAlreadyRegistered);

        var temporaryPassword = Convert.ToHexString(RandomNumberGenerator.GetBytes(8)).ToLowerInvariant();
        var employee = new User(command.Name, email, hashingService.HashPassword(temporaryPassword),
            UserRoles.Employee, command.CompanyId, command.Position);

        try
        {
            await emailService.SendAsync(employee.Email, "CompuCare invitation",
                $"Your temporary password is: {temporaryPassword}. Change it after your first sign-in.",
                cancellationToken);
        }
        catch (InvalidOperationException)
        {
            return Result<(User employee, string? temporaryPassword)>.Failure(IamErrors.EmailDeliveryUnavailable,
                IamError.EmailDeliveryUnavailable);
        }

        await userRepository.AddAsync(employee, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<(User employee, string? temporaryPassword)>.Success(
            (employee, emailService.IsSimulated ? temporaryPassword : null));
    }

    /// <inheritdoc />
    public async Task<Result<User>> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken)
    {
        if (command.UserId != command.RequesterId)
            return Result<User>.Failure(IamErrors.UserUpdateForbidden, IamError.UserUpdateForbidden);

        var user = await userRepository.FindByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result<User>.Failure(IamErrors.UserNotFound, IamError.UserNotFound);

        try
        {
            user.UpdateProfile(command.Name, command.Phone);
        }
        catch (ArgumentException)
        {
            return Result<User>.Failure(IamErrors.InvalidUserData, IamError.InvalidUserData);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<User>.Success(user);
    }

    /// <inheritdoc />
    public async Task<Result> Handle(ChangeUserPasswordCommand command, CancellationToken cancellationToken)
    {
        if (command.UserId != command.RequesterId)
            return Result.Failure(IamErrors.UserUpdateForbidden, IamError.UserUpdateForbidden);

        var user = await userRepository.FindByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(IamErrors.UserNotFound, IamError.UserNotFound);

        if (!hashingService.VerifyPassword(command.CurrentPassword ?? string.Empty, user.PasswordHash))
            return Result.Failure(IamErrors.CurrentPasswordMismatch, IamError.CurrentPasswordMismatch);

        if (command.NewPassword is null || command.NewPassword.Length < MinimumPasswordLength)
            return Result.Failure(IamErrors.InvalidPassword, IamError.InvalidPassword);

        user.UpdatePasswordHash(hashingService.HashPassword(command.NewPassword));
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
