using Kite.Domain.Entities;
using Kite.Domain.Enums;

namespace Kite.Domain.Interfaces;

public interface IApplicationFileRepository : IGenericRepository<ApplicationFile, Guid>
{
    Task<IEnumerable<ApplicationFile>> GetUserFilesByTypeAsync(string userId, FileType type, CancellationToken cancellationToken = default);
    Task<ApplicationFile?> GetLatestUserFileByTypeAsync(string userId, FileType type, CancellationToken cancellationToken = default);
}