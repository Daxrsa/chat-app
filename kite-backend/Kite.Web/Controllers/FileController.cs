using Kite.Application.Interfaces;
using Kite.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class FileController(IFileUploaderService fileUploaderService) : BaseApiController
{
    [HttpPost("upload-file")]
    public async Task<IActionResult> UploadFileAsync(IFormFile file, FileType type, CancellationToken cancellationToken)
        => HandleResult(await fileUploaderService.UploadFileAsync(file,type, cancellationToken));
    
    // needs more testing
    [HttpPost("upload-files")]
    public async Task<IActionResult> UploadFilesAsync(IFormFileCollection files, FileType type, CancellationToken cancellationToken)
        => HandleResult(await fileUploaderService.UploadFilesAsync(files, type, cancellationToken));
}