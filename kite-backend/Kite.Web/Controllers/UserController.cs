using Kite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserAccessor _userAccessor;
    private readonly IAuthService _authService;

    public UserController(IUserAccessor userAccessor, IAuthService authService)
    {
        _userAccessor = userAccessor;
        _authService = authService;
    }

    [HttpGet("get-logged-in-user")]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        return Ok(await _userAccessor.GetCurrentUserAsync());
    }

    [HttpDelete("delete-user-by-email")]
    public async Task<IActionResult> DeleteUserByEmail(string email)
    {
        return Ok(await _authService.DeleteUserByEmail(email));
    }
}