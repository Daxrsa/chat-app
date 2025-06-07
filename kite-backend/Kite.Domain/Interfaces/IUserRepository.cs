using Kite.Domain.Entities;

namespace Kite.Domain.Interfaces;

public interface IUserRepository : IGenericRepository<ApplicationUser, string>
{
    
}