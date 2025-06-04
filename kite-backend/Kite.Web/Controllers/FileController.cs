using Kite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class FileController(IFileUploaderService fileUploaderService) : BaseApiController
{
    [HttpPost("upload-file")]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
        => HandleResult(await fileUploaderService.UploadFileAsync(file, cancellationToken));
}