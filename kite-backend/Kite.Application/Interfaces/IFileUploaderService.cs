using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Kite.Application.Interfaces;

public interface IFileUploaderService
{
    Task<Result<FileUploadResult>> UploadFileAsync(IFormFile file, FileType type, CancellationToken cancellationToken);
    Task<Result<BatchUploadResult>> UploadFilesAsync(IFormFileCollection files, FileType type, CancellationToken cancellationToken);
}