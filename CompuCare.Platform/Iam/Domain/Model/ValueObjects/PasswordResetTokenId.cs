using CompuCare.Platform.Shared.Domain.Model.ValueObjects;

namespace CompuCare.Platform.Iam.Domain.Model.ValueObjects;

/// <summary>
///     Represents the strongly-typed identifier for a password reset token.
/// </summary>
/// <param name="Value">The underlying identifier value.</param>
public readonly record struct PasswordResetTokenId(int Value) : IEntityId<int>
{
    public static implicit operator int(PasswordResetTokenId id) => id.Value;
    public static implicit operator PasswordResetTokenId(int value) => new(value);

    public override string ToString() => Value.ToString();
}
