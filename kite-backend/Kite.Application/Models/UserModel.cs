namespace Kite.Application.Models;

public class UserModel
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string UserName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public string Role { get; set; }
    public string ImageUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}