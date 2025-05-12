using Kite.Domain.Entities;

namespace Kite.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateJwtTokenAsync(ApplicationUser user);
}