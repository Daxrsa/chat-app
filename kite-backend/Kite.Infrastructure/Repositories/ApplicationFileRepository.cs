using Kite.Domain.Entities;
using Kite.Domain.Interfaces;

namespace Kite.Infrastructure.Repositories;

public class ApplicationFileRepository : GenericRepository<ApplicationFile, Guid>, IApplicationFileRepository
{
    public ApplicationFileRepository(AppDbContext context) : base(context)
    {
    }
}