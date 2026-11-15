using CompuCare.Platform.Iam.Domain.Model.Aggregates;
using CompuCare.Platform.Iam.Domain.Model.Queries;

namespace CompuCare.Platform.Iam.Application.QueryServices;

/// <summary>
///     The user query service.
/// </summary>
public interface IUserQueryService
{
    Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken);

    Task<IEnumerable<User>> Handle(GetEmployeesByCompanyIdQuery query, CancellationToken cancellationToken);

    Task<IEnumerable<User>> Handle(GetAllActiveTechniciansQuery query, CancellationToken cancellationToken);
}
