using CompuCare.Platform.Iam.Domain.Model.Aggregates;
using CompuCare.Platform.Iam.Domain.Model.Commands;
using CompuCare.Platform.Shared.Application.Model;

namespace CompuCare.Platform.Iam.Application.CommandServices;

/// <summary>
///     The user command service.
/// </summary>
public interface IUserCommandService
{
    Task<Result<(User user, string token)>> Handle(SignInCommand command, CancellationToken cancellationToken);

    Task<Result<(User admin, string companyName, string companyTaxId)>> Handle(SignUpCompanyCommand command,
        CancellationToken cancellationToken);

    Task<Result<(User employee, string? temporaryPassword)>> Handle(CreateEmployeeCommand command,
        CancellationToken cancellationToken);

    Task<Result<User>> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken);

    Task<Result> Handle(ChangeUserPasswordCommand command, CancellationToken cancellationToken);
}
