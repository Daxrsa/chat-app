namespace Kite.Domain.Common;

public static class UserErrors
{
    public static Error NotFound(string email) =>
        new Error("Users.NotFound", $"User with email {email} was not found.");

    public static Error NoChangesDetected =>
        new Error("Users.NoChanges", "No changes were detected during the operation.");

    public static Error Unauthorized =>
        new Error("Users.Unauthorized", "User is not authorized to perform this action.");

    public static Error CreationFailed => new Error("Users.CreationFailed",
        "User creation failed. No changes were made to the database.");

    public static Error UserAlreadyExists(string email) => new Error("Users.UserAlreadyExists",
        $"An account with the email address {email} already exists.");

    public static Error CreationUnexpectedError => new Error("Users.CreationUnexpectedError",
        "An unexpected error occurred during user creation.");

    public static Error RetrievalError =>
        new Error("Users.RetrievalError", "An error occurred while retrieving the list of users.");

    public static Error UpdateUnexpectedError => new Error("Users.UpdateUnexpectedError",
        "An unexpected error occurred during the update operation.");

    public static Error DeletionUnexpectedError => new Error("Users.DeletionUnexpectedError",
        "An unexpected error occurred during the deletion operation.");

    public static Error IncorrectEmailOrPassword => new Error("Users.Error", "Incorrect email or password.");

    public static Error ClaimFailed => new Error("Users.Claim", "Failed to assign claims to the user.");

    public static Error UnexpectedError => new Error("Users.UnexpectedError", "An unexpected error occurred.");

    public static Error TokenCreationFailed => new Error("Users.TokenCreationFailed",
        "An error occurred while generating the authentication token.");

    public static Error UnverifiedEmail => new Error("Users.UnverifiedEmail",
        "Email address is not verified.");
}