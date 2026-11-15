using CompuCare.Platform.Iam.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Domain.Model.Aggregates;

namespace CompuCare.Platform.Iam.Domain.Model.Aggregates;

/// <summary>
///     One-time password reset token. Only a SHA-256 digest of the token is persisted.
/// </summary>
public class PasswordResetToken : AggregateRoot
{
    public PasswordResetToken()
    {
        TokenHash = string.Empty;
    }

    public PasswordResetToken(int userId, string tokenHash, DateTime expiresAt) : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);
        if (userId <= 0)
            throw new ArgumentException("A valid user is required.", nameof(userId));
        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("The expiration must be in the future.", nameof(expiresAt));
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
    }

    public PasswordResetTokenId Id { get; }
    public int UserId { get; private set; }
    public string TokenHash { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }

    public bool IsUsable => UsedAt is null && ExpiresAt > DateTime.UtcNow;

    public void Consume()
    {
        if (!IsUsable)
            throw new InvalidOperationException("Password reset token expired or already used.");
        UsedAt = DateTime.UtcNow;
    }
}
