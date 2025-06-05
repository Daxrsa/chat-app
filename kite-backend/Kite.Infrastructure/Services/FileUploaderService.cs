using System.Text.Encodings.Web;
using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Application.Utilities;
using Kite.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kite.Infrastructure.Services;

public class FileUploaderService(
    IUserAccessor userAccessor,
    IAntivirusService antivirusService) : IFileUploaderService
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
        try
        {
            // var currentUserId = userAccessor.GetCurrentUserId();

            if (file == null || file.Length == 0)
                return Result<FileUploadResult>.Failure(FileUploadErrors.NoFile);

            if (file.Length > FileSizeLimit)
                return Result<FileUploadResult>.Failure(
                    FileUploadErrors.SizeExceededWithLimit(Helpers.FormatFileSize(FileSizeLimit)));
            
            var avScan = await antivirusService.ScanFileAsync(file);
        
            if (!avScan.IsSuccess)
            {
                return Result<FileUploadResult>.Failure(avScan.Errors.First().Description);
            }
        
            if (!avScan.Value.IsClean)
            {
                return Result<FileUploadResult>.Failure(
                    FileUploadErrors.VirusDetected(avScan.Value.VirusName));
            }

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
                FileSize = Helpers.FormatFileSize(file.Length),
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

    public async Task<Result<BatchUploadResult>> UploadFilesAsync(
        IFormFileCollection files,
        CancellationToken cancellationToken)
    {
        try
        {
            var uploadStartTime = DateTime.UtcNow;

            if (files == null || !files.Any())
                return Result<BatchUploadResult>.Failure(FileUploadErrors.NoFile);

            var successfulUploads = new List<FileUploadResult>();
            var failedUploads = new List<FailedUpload>();

            var fileIndex = 0;
            foreach (var file in files)
            {
                fileIndex++;

                try
                {
                    var uploadResult = await UploadFileAsync(file, cancellationToken);

                    if (uploadResult.IsSuccess)
                    {
                        successfulUploads.Add(uploadResult.Value);
                    }
                    else
                    {
                        var failedUpload = new FailedUpload
                        {
                            FileName = file.FileName ?? "unknown",
                            Reason = uploadResult.Errors.FirstOrDefault()?.Description ??
                                     "Unknown error"
                        };
                        failedUploads.Add(failedUpload);
                    }
                }
                catch (Exception ex)
                {
                    var failedUpload = new FailedUpload
                    {
                        FileName = file.FileName ?? "unknown",
                        Reason = $"Unexpected error during upload - {ex.Message}"
                    };
                    failedUploads.Add(failedUpload);
                }
            }

            var uploadEndTime = DateTime.UtcNow;
            var totalDuration = (uploadEndTime - uploadStartTime).TotalMilliseconds;

            var batchResult = new BatchUploadResult
            {
                TotalFiles = files.Count,
                SuccessfulUploads = successfulUploads,
                FailedUploads = failedUploads,
                SuccessCount = successfulUploads.Count,
                FailureCount = failedUploads.Count,
                IsPartialSuccess = failedUploads.Count > 0 && successfulUploads.Count > 0,
                UploadedAt = uploadStartTime,
                CompletedAt = uploadEndTime,
                UploadDurationMs = totalDuration,
                UploadedBy = ""
            };

            if (successfulUploads.Count > 0)
            {
                return Result<BatchUploadResult>.Success(batchResult);
            }

            return Result<BatchUploadResult>.Failure(
                $"{files.Count} file/s failed to upload. See FailedUploads for details.");
        }
        catch (Exception ex)
        {
            return Result<BatchUploadResult>.Failure(
                $"Critical error during batch upload: {ex.Message}");
        }
    }
}

// public async Task RemoveExecutePermissionsAsync(string filePath)
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