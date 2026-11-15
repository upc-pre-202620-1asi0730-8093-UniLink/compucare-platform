namespace CompuCare.Platform.Iam.Domain.Model.Commands;

/// <summary>
///     Updates the contact data of a user. Only the user itself can perform it.
/// </summary>
public record UpdateUserProfileCommand(int UserId, int RequesterId, string Name, string? Phone);
