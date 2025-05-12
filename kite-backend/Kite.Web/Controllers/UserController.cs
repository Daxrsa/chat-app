using Kite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserAccessor _userAccessor;

    public UserController(IUserAccessor userAccessor)
    {
        _userAccessor = userAccessor;
    }

    [HttpGet("GetCurrentUserAsync")]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        return Ok(await _userAccessor.GetCurrentUserAsync());
    }
}