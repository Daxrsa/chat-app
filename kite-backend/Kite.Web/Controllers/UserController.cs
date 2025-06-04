using Kite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class UserController(IUserAccessor userAccessor, IAuthService authService)
    : BaseApiController
{
    // [HttpGet("get-logged-in-user")]
    // public async Task<IActionResult> GetCurrentUserAsync()
    //     => HandleResult(await userAccessor.GetCurrentUserAsync());
    
    [HttpGet("get-logged-in-user")]
    public IActionResult GetCurrentUser()
    {
        // For non-authenticated endpoints, return a default or guest user
        return Ok(new { 
            Firstname = "Guest",
            Lastname = "Doe",
            IsAuthenticated = false
        });
    }

    [HttpDelete("delete-user-by-email")]
    public async Task<IActionResult> DeleteUserByEmail(string email)
        => HandleResult(await authService.DeleteUserByEmail(email));
    
    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsersAsync()
        => HandleResult(await userAccessor.GetAllUsersAsync());
}