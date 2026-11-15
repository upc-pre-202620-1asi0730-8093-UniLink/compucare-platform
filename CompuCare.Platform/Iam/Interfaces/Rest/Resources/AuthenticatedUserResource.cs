namespace CompuCare.Platform.Iam.Interfaces.Rest.Resources;

public record AuthenticatedUserResource(string Token, string TokenType, UserResource User);
