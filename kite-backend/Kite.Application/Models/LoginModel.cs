namespace Kite.Application.Models;

public class LoginModel
{
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public required string Password { get; set; }
}