using System.Text.Encodings.Web;
using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kite.Infrastructure.Services;

public class FileUploaderService(ILogger<FileUploaderService> logger) : IFileUploaderService
{
    private static readonly string[] PermittedExtensions =
        { ".txt", ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".mp3", ".mp4", ".webp" };

    // 15 MB limit
    private const long FileSizeLimit = 15 * 1024 * 1024;

    private static readonly string
        TargetFilePath = Path.Combine("/home/daorsa/Desktop/KiteUploads");

    private static readonly Dictionary<string, string> FileTypeToFolder = new()
    {
        { ".jpg", "images" },
        { ".jpeg", "images" },
        { ".png", "images" },
        { ".gif", "images" },
        { ".webp", "images" },
        { ".mp3", "audio" },
        { ".mp4", "video" },
        { ".txt", "documents" },
        { ".pdf", "documents" }
    };

    public async Task<Result<FileUploadResult>> UploadFileAsync(IFormFile file,
        CancellationToken cancellationToken)
    {
        // Virus scan placeholder (implement with your AV solution)
        // var isClean = await MyAntivirusScanner.IsCleanAsync(file.OpenReadStream());
        // if (!isClean) return Result<FileUploadResult>.Failure("FileUpload.VirusScan", "File failed virus scan.");

        try
        {
            if (file == null || file.Length == 0)
                return Result<FileUploadResult>.Failure(FileUploadErrors.NoFile);

            if (file.Length > FileSizeLimit)
                return Result<FileUploadResult>.Failure(
                    FileUploadErrors.SizeExceededWithLimit(FormatFileSize(FileSizeLimit)));

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !PermittedExtensions.Contains(ext))
                return Result<FileUploadResult>.Failure(
                    FileUploadErrors.InvalidExtensionWithAllowed(PermittedExtensions));

            var subfolder = FileTypeToFolder.GetValueOrDefault(ext, "others");
            var targetDirectory = Path.Combine(TargetFilePath, subfolder);

            Directory.CreateDirectory(targetDirectory);

            var safeFileName = $"{Guid.NewGuid()}{ext}";
            var finalPath = Path.Combine(targetDirectory, safeFileName);

            await using (var stream = new FileStream(finalPath, FileMode.Create))
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
                FileSize = FormatFileSize(file.Length),
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

    public Task<Result<List<FileUploadResult>>> UploadMultipleFilesAsync(IFormFileCollection files, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static string FormatFileSize(long sizeInBytes)
    {
        if (sizeInBytes == 0)
            return "0 B";

        string[] units = { "B", "KB", "MB", "GB", "TB", "PB" };
        double size = sizeInBytes;
        int unitIndex = 0;

        while (size >= 1024 && unitIndex < units.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return unitIndex == 0
            ? $"{size:F0} {units[unitIndex]}"
            : $"{size:F2} {units[unitIndex]}";
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