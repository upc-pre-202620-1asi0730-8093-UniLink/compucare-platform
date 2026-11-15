using CompuCare.Platform.Iam.Domain.Model.Aggregates;

namespace CompuCare.Platform.Iam.Application.Internal.OutboundServices;

/// <summary>
///     Generates and validates JWT tokens.
/// </summary>
public interface ITokenService
{
    string GenerateToken(User user);
    Task<int?> ValidateToken(string token);
}
