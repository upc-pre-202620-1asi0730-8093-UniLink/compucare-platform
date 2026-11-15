using CompuCare.Platform.Iam.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Domain.Model.Aggregates;

namespace CompuCare.Platform.Iam.Domain.Model.Aggregates;

/// <summary>
///     The user aggregate. Holds the account identity, its role and its company membership.
/// </summary>
public partial class User : AggregateRoot
{
    public User()
    {
        Name = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
        Role = string.Empty;
        Phone = string.Empty;
        Position = string.Empty;
    }

    public User(string name, string email, string passwordHash, string role, int? companyId = null,
        string? position = null) : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        if (!UserRoles.IsKnown(role))
            throw new ArgumentException("Unrecognized account role.", nameof(role));
        if (UserRoles.RequiresCompany(role) && companyId is null)
            throw new ArgumentException("Company users must belong to a company.", nameof(companyId));

        Name = name.Trim();
        Email = NormalizeEmail(email);
        PasswordHash = passwordHash;
        Role = role;
        CompanyId = companyId;
        Position = position?.Trim() ?? string.Empty;
        Active = true;
    }

    public UserId Id { get; }
    public int? CompanyId { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; }
    public string Phone { get; private set; }
    public string Position { get; private set; }
    public bool Active { get; private set; }

    /// <summary>
    ///     Normalizes an email address the same way it is persisted, so lookups are case-insensitive.
    /// </summary>
    public static string NormalizeEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        var normalized = email.Trim().ToLowerInvariant();
        if (!normalized.Contains('@'))
            throw new ArgumentException("A valid email address is required.", nameof(email));
        return normalized;
    }

    public User UpdateProfile(string name, string? phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        Phone = phone?.Trim() ?? string.Empty;
        return this;
    }

    public User UpdatePasswordHash(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
        return this;
    }
}
