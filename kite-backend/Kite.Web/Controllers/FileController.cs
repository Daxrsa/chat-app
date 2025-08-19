using Kite.Application.Interfaces;
using Kite.Application.Utilities;
using Kite.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class FileController(IFileUploaderService fileUploaderService) : BaseApiController
{
    [HttpGet("serve-file/{fileId:guid}")]
    public async Task<IActionResult> ServeFile(Guid fileId, CancellationToken cancellationToken)
    {
        var result = await fileUploaderService.ServeFileAsync(fileId, cancellationToken);

        if (!result.IsSuccess)
            return HandleResult(result);

        var fileModel = result.Value;
    //jkn
        if (!System.IO.File.Exists(fileModel.FilePath))
            return NotFound("File not found on disk");
    
        var fileBytes = await System.IO.File.ReadAllBytesAsync(fileModel.FilePath, cancellationToken);
        var contentType = Helpers.GetContentType(fileModel.Extension) ?? "application/octet-stream";
        
        Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileModel.FileName}\"");
    
        return File(fileBytes, contentType);
    }
    
    [HttpGet("get-files")]
    public async Task<IActionResult> GetAllFiles(CancellationToken cancellationToken)
        => HandleResult(await fileUploaderService.GetAllFilesAsync(cancellationToken));

    [HttpPost("upload-file")]
    public async Task<IActionResult> UploadFile(IFormFile file, FileType type,
        CancellationToken cancellationToken)
        => HandleResult(await fileUploaderService.UploadFileAsync(file, type, cancellationToken));

    [HttpPost("upload-files")]
    public async Task<IActionResult> UploadFiles(IFormFileCollection files, FileType type,
        CancellationToken cancellationToken)
        => HandleResult(await fileUploaderService.UploadFilesAsync(files, type, cancellationToken));

    [HttpDelete("{fileId:guid}")]
    public async Task<IActionResult> DeleteFile(Guid fileId, CancellationToken cancellationToken)
        => HandleResult(await fileUploaderService.DeleteFileAsync(fileId, cancellationToken));

    [HttpPost("upload-profile-picture")]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file,
        CancellationToken cancellationToken)
        => HandleResult(await fileUploaderService.UploadFileAsync(file, FileType.ProfilePicture,
            cancellationToken));
}