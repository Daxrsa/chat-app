using Kite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class FileController(IFileUploaderService fileUploaderService) : BaseApiController
{
    [HttpPost("upload-file")]
    public async Task<IActionResult> UploadFileAsync(IFormFile file, CancellationToken cancellationToken)
        => HandleResult(await fileUploaderService.UploadFileAsync(file, cancellationToken));
    
    // needs more testing
    [HttpPost("upload-files")]
    public async Task<IActionResult> UploadFilesAsync(IFormFileCollection files, CancellationToken cancellationToken)
        => HandleResult(await fileUploaderService.UploadFilesAsync(files, cancellationToken));
}