using Kite.Application.Models;
using Kite.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Kite.Application.Interfaces;

public interface IFileUploaderService
{
    Task<Result<FileUploadResult>> UploadFileAsync(IFormFile file, CancellationToken cancellationToken);
    Task<Result<BatchUploadResult>> UploadFilesAsync(IFormFileCollection files, CancellationToken cancellationToken);
}