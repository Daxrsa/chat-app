using Kite.Domain.Entities;
using Kite.Domain.Interfaces;

namespace Kite.Infrastructure.Repositories;

public class UserRepository : GenericRepository<ApplicationUser, string>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
}