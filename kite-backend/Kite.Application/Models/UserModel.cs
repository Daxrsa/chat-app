namespace Kite.Application.Models;

public class UserModel
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}