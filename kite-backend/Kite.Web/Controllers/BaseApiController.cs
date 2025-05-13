using Kite.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return result.Code switch
        {
            // 4xx Client Errors
            400 => BadRequest(result.Errors),                       // Bad Request
            401 => Unauthorized(result.Errors),                     // Unauthorized
            403 => Forbid(),                                        // Forbidden
            404 => NotFound(result.Errors),                         // Not Found
            405 => StatusCode(405, result.Errors),                  // Method Not Allowed
            406 => StatusCode(406, result.Errors),                  // Not Acceptable
            409 => Conflict(result.Errors),                         // Conflict
            410 => StatusCode(410, result.Errors),                  // Gone
            412 => StatusCode(412, result.Errors),                  // Precondition Failed
            413 => StatusCode(413, result.Errors),                  // Payload Too Large
            415 => StatusCode(415, result.Errors),                  // Unsupported Media Type
            422 => UnprocessableEntity(result.Errors),              // Unprocessable Entity
            423 => StatusCode(423, result.Errors),                  // Locked
            429 => StatusCode(429, result.Errors),                  // Too Many Requests
        
            // 5xx Server Errors
            500 => StatusCode(500, result.Errors),                  // Internal Server Error
            501 => StatusCode(501, result.Errors),                  // Not Implemented
            502 => StatusCode(502, result.Errors),                  // Bad Gateway
            503 => StatusCode(503, result.Errors),                  // Service Unavailable
            504 => StatusCode(504, result.Errors),                  // Gateway Timeout
        
            // Default for any other codes
            _ => BadRequest(result.Errors)                          // Default to BadRequest for unhandled codes
        };
    }
}