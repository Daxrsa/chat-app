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

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        return Ok(await _authService.Register(model));
    }

    [HttpDelete("delete-user-by-email")]
    public async Task<IActionResult> DeleteUserByEmail(string email)
    {
        return Ok(await _authService.DeleteUserByEmail(email));
    }
}