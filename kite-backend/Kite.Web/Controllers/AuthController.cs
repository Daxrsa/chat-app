using Kite.Application.Interfaces;
using Kite.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : BaseApiController
{
    [HttpPost("register-user")]
    public async Task<IActionResult> Register(RegisterModel model)
        => HandleResult(await authService.RegisterAsync(model));

    [HttpPost("login-user")]
    public async Task<IActionResult> Login(LoginModel model)
        => HandleResult(await authService.LoginAsync(model));
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
        => HandleResult(await authService.LogoutAsync());
}