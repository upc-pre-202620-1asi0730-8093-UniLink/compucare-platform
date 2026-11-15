using CompuCare.Platform.Iam.Application.QueryServices;
using CompuCare.Platform.Iam.Domain.Model.Aggregates;
using CompuCare.Platform.Iam.Domain.Model.Queries;
using CompuCare.Platform.Iam.Domain.Model.ValueObjects;
using CompuCare.Platform.Iam.Domain.Repositories;

namespace CompuCare.Platform.Iam.Application.Internal.QueryServices;

public class UserQueryService(IUserRepository userRepository) : IUserQueryService
{
    /// <inheritdoc />
    public async Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        return await userRepository.FindByIdAsNoTrackingAsync(query.Id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> Handle(GetEmployeesByCompanyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await userRepository.FindByCompanyIdAndRoleAsync(query.CompanyId, UserRoles.Employee,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> Handle(GetAllActiveTechniciansQuery query,
        CancellationToken cancellationToken)
    {
        return await userRepository.FindActiveByRoleAsync(UserRoles.Technician, cancellationToken);
    }
}
