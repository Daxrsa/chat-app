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
    
    [HttpGet("get-logged-in-user")]
    public async Task<IActionResult> GetCurrentUserAsync(CancellationToken cancellationToken)
        => HandleResult(await authService.GetCurrentUserAsync(cancellationToken));
    
    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken)
        => HandleResult(await authService.GetAllUsersAsync(cancellationToken));
    
    [HttpDelete("delete-user-by-email")]
    public async Task<IActionResult> DeleteUserByEmail(string email)
        => HandleResult(await authService.DeleteUserByEmail(email));
}