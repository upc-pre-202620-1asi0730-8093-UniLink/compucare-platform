namespace CompuCare.Platform.Iam.Interfaces.Rest.Resources;

public record ChangeUserPasswordResource(string CurrentPassword, string NewPassword);
