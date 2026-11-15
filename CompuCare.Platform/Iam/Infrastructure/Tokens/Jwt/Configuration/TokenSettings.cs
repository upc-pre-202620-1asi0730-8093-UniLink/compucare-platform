namespace CompuCare.Platform.Iam.Infrastructure.Tokens.Jwt.Configuration;

/// <summary>
///     Binds the <c>TokenSettings</c> configuration section used to sign JWTs.
/// </summary>
public class TokenSettings
{
    public required string Secret { get; set; }
}
