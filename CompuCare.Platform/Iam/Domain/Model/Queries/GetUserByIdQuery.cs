using CompuCare.Platform.Iam.Domain.Model.ValueObjects;

namespace CompuCare.Platform.Iam.Domain.Model.Queries;

/// <summary>
///     The get user by id query.
/// </summary>
public record GetUserByIdQuery(UserId Id)
{
    public GetUserByIdQuery(int id) : this(new UserId(id))
    {
    }
}
