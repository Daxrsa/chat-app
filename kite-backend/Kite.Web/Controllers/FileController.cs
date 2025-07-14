using Kite.Application.Interfaces;
using Kite.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class FileController(IFileUploaderService fileUploaderService) : BaseApiController
{
    [HttpPost("upload-file")]
    public async Task<IActionResult> UploadFile(IFormFile file, FileType type, CancellationToken cancellationToken)
        => HandleResult(await fileUploaderService.UploadFileAsync(file,type, cancellationToken));
    
    [HttpPost("upload-files")]
    public async Task<IActionResult> UploadFiles(IFormFileCollection files, FileType type, CancellationToken cancellationToken)
        => HandleResult(await fileUploaderService.UploadFilesAsync(files, type, cancellationToken));
    
    [HttpDelete("{fileId:guid}")]
    public async Task<IActionResult> DeleteFile(Guid fileId, CancellationToken cancellationToken)
        => HandleResult(await fileUploaderService.DeleteFileAsync(fileId, cancellationToken));
    
    [HttpPost("upload-profile-picture")]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file, CancellationToken cancellationToken)
        => HandleResult(await fileUploaderService.UploadFileAsync(file, FileType.ProfilePicture, cancellationToken));
}