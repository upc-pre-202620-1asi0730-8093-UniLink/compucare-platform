using CompuCare.Platform.Iam.Application.Internal.OutboundServices;
using BCryptNet = BCrypt.Net.BCrypt;

namespace CompuCare.Platform.Iam.Infrastructure.Hashing.BCrypt.Services;

/// <summary>
///     Hashes and verifies passwords using BCrypt.
/// </summary>
public class HashingService : IHashingService
{
    public string HashPassword(string password)
    {
        return BCryptNet.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            return BCryptNet.Verify(password, passwordHash);
        }
        catch (global::BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}
