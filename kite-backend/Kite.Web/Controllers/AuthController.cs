using Kite.Application.Interfaces;
using Kite.Application.Models;
using Microsoft.AspNetCore.Authorization;
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
    
    [HttpPost("register-user")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        return Ok(await _authService.RegisterAsync(model));
    }
    
    [HttpPost("login-user")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        return Ok(await _authService.LoginAsync(model));
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        return Ok(await _authService.LogoutAsync());
    }
}