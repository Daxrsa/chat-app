using System.Text.Encodings.Web;
using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kite.Infrastructure.Services;

public class FileUploaderService(ILogger<FileUploaderService> logger) : IFileUploaderService
{
    private static readonly string[] _permittedExtensions = { ".txt", ".pdf", ".jpg", ".jpeg", ".png", ".gif" };
    // 10 MB limit
    private const long _fileSizeLimit = 10 * 1024 * 1024;
    private static readonly string _targetFilePath = Path.Combine("/home/daorsa/Desktop/KiteUploads");

    public async Task<Result<FileUploadResult>> UploadFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        // Virus scan placeholder (implement with your AV solution)
        // var isClean = await MyAntivirusScanner.IsCleanAsync(file.OpenReadStream());
        // if (!isClean) return Result<FileUploadResult>.Failure("FileUpload.VirusScan", "File failed virus scan.");

        try
        {
            if (file == null || file.Length == 0)
                return Result<FileUploadResult>.Failure(FileUploadErrors.NoFile);
        
            if (file.Length > _fileSizeLimit)
                return Result<FileUploadResult>.Failure(FileUploadErrors.SizeExceeded);
        
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !_permittedExtensions.Contains(ext))
                return Result<FileUploadResult>.Failure(FileUploadErrors.InvalidExtension);
            
            // Ensure upload directory exists and has no execute permissions (set via OS)
            // Directory.CreateDirectory(_targetFilePath);
            
            var safeFileName = $"{Guid.NewGuid()}{ext}";
            var finalPath = Path.Combine(_targetFilePath, safeFileName);

            using (var stream = new FileStream(finalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }
            
            // await RemoveExecutePermissionsAsync(finalPath);
            
            var encodedOriginalName = HtmlEncoder.Default.Encode(Path.GetFileName(file.FileName));
            var result = new FileUploadResult
            {
                Message = "File uploaded successfully.",
                OriginalFileName = encodedOriginalName,
                StoredFileName = safeFileName,
                FilePath = finalPath,
                FileSize = file.Length,
                UploadedAt = DateTime.UtcNow
            };

            return Result<FileUploadResult>.Success(result);
        }
        catch (DirectoryNotFoundException)
        {
            return Result<FileUploadResult>.Failure(FileUploadErrors.DirectoryNotFound);
        }
        catch (UnauthorizedAccessException)
        {
            return Result<FileUploadResult>.Failure(FileUploadErrors.AccessDenied);
        }
        catch (IOException ex)
        {
            return Result<FileUploadResult>.Failure(FileUploadErrors.IOError(ex.Message));
        }
        catch (Exception ex)
        {
            return Result<FileUploadResult>.Failure(FileUploadErrors.UnknownError(ex.Message));
        }
    }

    // private async Task RemoveExecutePermissionsAsync(string filePath)
    // {
    //     try
    //     {
    //         // Remove execute permissions on Linux (chmod 644)
    //         var process = new Process
    //         {
    //             StartInfo = new ProcessStartInfo
    //             {
    //                 FileName = "chmod",
    //                 Arguments = $"644 \"{filePath}\"",
    //                 UseShellExecute = false,
    //                 CreateNoWindow = true,
    //                 RedirectStandardOutput = true,
    //                 RedirectStandardError = true
    //             }
    //         };
    //
    //         process.Start();
    //         await process.WaitForExitAsync();
    //         
    //         if (process.ExitCode != 0)
    //         {
    //             var error = await process.StandardError.ReadToEndAsync();
    //             logger.LogWarning($"Failed to remove execute permissions from {filePath}: {error}");
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         logger.LogWarning($"Exception while removing execute permissions from {filePath}: {ex.Message}");
    //     }
    // }
}