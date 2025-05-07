using Kite.Application.Interfaces;
using Kite.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel request)
    {
        return Ok(await _authService.LoginAsync(request));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel request)
    {
        return Ok(await _authService.RegisterAsync(request));
    }
    
    [HttpDelete("delete-user-by-email")]
    public async Task<IActionResult> DeleteUser([FromQuery] string email)
    {
        return Ok(await _authService.DeleteUserByEmailAsync(email));
    }
}